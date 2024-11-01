
GO
/****** Object:  StoredProcedure [dbo].[prcPlanoZeroGerarPedidosCliente]    Script Date: 06/03/2024 15:26:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER Procedure [dbo].[prcPlanoZeroGerarPedidosCliente]
	@ID_USUARIO int ,
	@idControlePlanoZero int 
AS
BEGIN
	SET NOCOUNT ON
	Delete from tbPlanoZeroClienteV2
	--Delete from tbPlanoZeroTecnico
	--SET NOCOUNT ON
	--Delete from tbPotencialPecasV2

  DECLARE 
  --@ID_USUARIO INT = 1, --REMOVER 
    @cursor CURSOR,						      @FATORPONDERACAO int,
    @cursor2 CURSOR,					      @POTENCIALPECA int,
    @QTDPEDIDOPZ int,					      --@QTDPZACALCULADAFLOAT float,
	@QTDPZACALCULADA float,				      @QTDULTIMOANO int,
    @PZPERIODO int,						  	  @p_TOKEN_GERADO bigint,
    @QTD_MINIMA int,					      @QT_PZ_PERIODO int,
    @QTD_PEDIDO int,					      @p_ID_PEDIDO numeric(9, 0),
	@CD_GRUPO_MODELO varchar(15),		      @p_TOKEN bigint,
    @CD_PECA varchar(15),				      @p_ID_ITEM numeric(9, 0),
    @QT_PECA int,						      @p_Data datetime = GETDATE(),
    @MAQ_CART int,						      @MAQ_CART_TEC int,
	@cd_cliente numeric(6, 0),
	@CD_TECNICO varchar(6),				      @estoque int,
    @QT_PECA_MODELO int,				  	  @cdsErrorMessage	NVARCHAR(4000),
    @QT_CLIENTES int,					  	  @nidErrorSeverity	INT,
	@MAXCLIENTES int,					  	  @nidErrorState INT,
	@QTPORMAQ int,							  @QT_ESTOQUECLI INT,
	@valPeca numeric(14,2),					  @CD_Pedido bigint,
	@POT_CLIENTE int,						  @PeriodoPZ int
	Begin --remover
	--Begin Try
		
	--Pega as peças do Plano Zero de Criticidade 'A'  e qtd por maq
	--SET NOCOUNT ON
	--SET @cursor = CURSOR FOR
	--SELECT	pz.CD_PECA, PZ.QT_PECA_MODELO, PZ.CD_GRUPO_MODELO
	--FROM tbPlanoZero pz
	--WHERE pz.CD_CRITICIDADE_ABC = 'A'
	----AND MONTH(pz.DT_CRIACAO) = MONTH(GETDATE()) -12
	----AND YEAR(pz.DT_CRIACAO) <= YEAR(GETDATE()) --REMOVER ANTES DE COMPILAR 
	--OPEN @cursor
	--	FETCH NEXT FROM @cursor INTO @CD_PECA, @QTPORMAQ, @CD_GRUPO_MODELO
	--		  WHILE @@FETCH_STATUS = 0
	--	BEGIN
	--		--Verifica a quantidade de maquinas EM CARTEIRA 
	--		SELECT
	--		@MAQ_CART = COUNT(*)
	--		FROM TB_ATIVO_CLIENTE ac
	--		INNER JOIN TB_ATIVO_FIXO af ON af.CD_ATIVO_FIXO = ac.CD_ATIVO_FIXO
	--		INNER JOIN TB_MODELO md ON md.CD_MODELO = af.CD_MODELO
	--		INNER JOIN tbPlanoZero pz ON pz.CD_GRUPO_MODELO = md.CD_GRUPO_MODELO
	--		INNER JOIN TB_PECA pc ON pc.CD_PECA = pz.CD_PECA
	--		WHERE 
	--		ac.DT_DEVOLUCAO IS NULL AND pz.CD_CRITICIDADE_ABC = 'A'
	--		AND md.CD_GRUPO_MODELO = @CD_GRUPO_MODELO
	--		and pc.CD_PECA = @CD_PECA
			
	--		--Cria o Potencial Total de Peças
	--		--BEGIN TRANSACTION
	--		--	SET NOCOUNT ON
	--		--	UPDATE tbPotencialPecasV2
	--		--		SET potencialPeca = potencialPeca + (@MAQ_CART * @QTPORMAQ)
	--		--		WHERE codPeca = @CD_PECA
	--		--	IF @@ROWCOUNT = 0
	--		--	BEGIN
	--		--		INSERT INTO tbPotencialPecasV2 (codPeca, potencialPeca)
	--		--		VALUES (@CD_PECA, @MAQ_CART * @QTPORMAQ)
	--		--	END
	--		--COMMIT TRANSACTION

	--	FETCH NEXT FROM @cursor INTO @CD_PECA, @QTPORMAQ, @CD_GRUPO_MODELO
	--	END
	--CLOSE @cursor
	--DEALLOCATE @cursor
	

	--pega a lista de técnicos que estão ativos e tem cliente sem plano zero 
	SET @cursor = CURSOR FOR
	SELECT distinct
	tt.CD_TECNICO, cl.CD_CLIENTE, cl.QTD_PeriodoPlanoZero
	FROM TB_TECNICO tt
	INNER JOIN TB_TECNICO_CLIENTE tc
	ON tc.CD_TECNICO = tt.CD_TECNICO
	INNER JOIN TB_CLIENTE cl
	ON cl.CD_CLIENTE = tc.CD_CLIENTE
	WHERE tt.FL_ATIVO = 'S'
	AND tc.CD_ORDEM = 1
	AND cl.FL_AtivaPlanoZero = 'S'
	AND cl.QTD_PeriodoPlanoZero is not null
	AND (cl.DT_ULTIMOPLANOZERO is null or cl.DT_ULTIMOPLANOZERO < (DATEADD(MONTH, cl.QTD_PeriodoPlanoZero, GETDATE())))
	order by tt.CD_TECNICO

	OPEN @cursor
		FETCH NEXT FROM @cursor INTO @CD_TECNICO, @CD_CLIENTE, @PeriodoPZ
			  WHILE @@FETCH_STATUS = 0
				BEGIN
					Set @cursor2 = CURSOR FOR
						SELECT PC.CD_PECA, pz.QT_PECA_MODELO, pc.QTD_PlanoZero, pz.CD_GRUPO_MODELO
						FROM 
						--TB_TECNICO tt
						--INNER JOIN TB_TECNICO_CLIENTE tc ON tc.CD_TECNICO = tt.CD_TECNICO
						--INNER JOIN
						TB_CLIENTE cl 
						INNER JOIN TB_ATIVO_CLIENTE ac ON ac.CD_CLIENTE = cl.CD_CLIENTE
						INNER JOIN TB_ATIVO_FIXO af ON af.CD_ATIVO_FIXO = ac.CD_ATIVO_FIXO
						INNER JOIN TB_MODELO md ON md.CD_MODELO = af.CD_MODELO
						INNER JOIN tbGrupoModelo gpmd on md.CD_GRUPO_MODELO = gpmd.cd_grupoModelo
						INNER JOIN tbPlanoZero pz ON pz.CD_GRUPO_MODELO = gpmd.cd_grupoModelo
						INNER JOIN TB_PECA pc ON pc.CD_PECA = pz.CD_PECA
						WHERE 
						cl.CD_CLIENTE = @cd_cliente
						--tc.CD_ORDEM = 1
						--AND tt.FL_ATIVO = 'S' AND cl.DT_DESATIVACAO IS NULL
						AND ac.DT_DEVOLUCAO IS NULL AND pz.CD_CRITICIDADE_ABC = 'A'
						AND FL_STATUS = 1
						AND cl.FL_AtivaPlanoZero = 'S' 
						--AND MONTH(pz.DT_CRIACAO) = MONTH(GETDATE())
						--AND YEAR(pz.DT_CRIACAO) = YEAR(GETDATE()) --REMOVER ANTES DE COMPILAR 
						--AND TC.CD_TECNICO = @CD_TECNICO and cl.CD_CLIENTE = @cd_cliente 
						ORDER BY PC.CD_PECA
					OPEN @cursor2
					FETCH NEXT FROM @cursor2 INTO @CD_PECA, @POT_CLIENTE, @QTD_MINIMA, @CD_GRUPO_MODELO
					WHILE @@FETCH_STATUS = 0
					BEGIN
						--pega o numero de clientes do técnico
						SELECT
						@QT_CLIENTES = COUNT(*)
						FROM TB_TECNICO tt
						INNER JOIN TB_TECNICO_CLIENTE tc
						ON tc.CD_TECNICO = tt.CD_TECNICO
						INNER JOIN TB_CLIENTE cl
						ON cl.CD_CLIENTE = tc.CD_CLIENTE
						WHERE tt.CD_TECNICO = @CD_TECNICO
						AND tt.FL_ATIVO = 'S'
						AND tc.CD_ORDEM = 1
						AND cl.FL_AtivaPlanoZero = 'S'
						
						--pega o fator de ponderação do tecnico
						Select top(1) @FATORPONDERACAO = fator 
						from TB_PONDERACAO_pz where MIN_CLIENTES <= @QT_CLIENTES and MAX_CLIENTES >= @QT_CLIENTES

						if (@FATORPONDERACAO is null)
							set @FATORPONDERACAO = 0

						--pega a quantidade do ultimo ano
						SELECT 
							@QTDULTIMOANO = isnull(Sum(tpc.QTD_RECEBIDA),0)
						FROM
							TB_PEDIDO tp
							INNER JOIN TB_PEDIDO_PECA tpc
							ON tp.ID_PEDIDO = tpc.ID_PEDIDO
							INNER JOIN TB_PECA pc
							ON tpc.CD_PECA = pc.CD_PECA
							
						WHERE
							(tp.DT_CRIACAO) > (DATEADD(day, -364, GETDATE()))
							and tp.DT_CRIACAO < (DATEADD(day, -1, GETDATE()))
							AND (tp.CD_CLIENTE		= @CD_CLIENTE)
							and tp.ID_STATUS_PEDIDO = 4
							and tpc.QTD_RECEBIDA is not null
							and tpc.CD_PECA = @CD_PECA
						

						--Pega o Potencial da Peça
						set @POTENCIALPECA = (select top 1 tpp.POTENCIALPECA from tbPotencialPecasV2 tpp where tpp.codPeca = @CD_PECA)
						
						--Paga a qtd de maq em carteira do tecnico
						SELECT
						@MAQ_CART_TEC = COUNT(*)
						FROM TB_ATIVO_CLIENTE ac
						INNER JOIN TB_CLIENTE tc ON tc.CD_CLIENTE = ac.CD_CLIENTE
						INNER JOIN TB_ATIVO_FIXO af ON af.CD_ATIVO_FIXO = ac.CD_ATIVO_FIXO
						INNER JOIN TB_MODELO md ON md.CD_MODELO = af.CD_MODELO
						INNER JOIN tbPlanoZero pz ON pz.CD_GRUPO_MODELO = md.CD_GRUPO_MODELO
						INNER JOIN TB_PECA pc ON pc.CD_PECA = pz.CD_PECA
						WHERE 
						ac.DT_DEVOLUCAO IS NULL AND pz.CD_CRITICIDADE_ABC = 'A'
						AND md.CD_GRUPO_MODELO = @CD_GRUPO_MODELO
						and pz.CD_PECA = @CD_PECA
						and tc.CD_CLIENTE = @CD_CLIENTE

							----Ajustar 
							declare @POTCalculo int = 0

							set @POTCalculo = Isnull((select top 1 potencialPecas from tbPlanoZeroClienteV2 where codCliente = @cd_cliente and codPeca = @CD_PECA),0)

							--Calcula o Plano Zero Anual
							SET @QTDPZACALCULADA = Isnull((CAST(@QTDULTIMOANO as float)/CAST(@POTENCIALPECA as float)) * Cast(@POT_CLIENTE as float),0)

							--if Cast(@QTDPZACALCULADA as float) > 0 and Cast(@QTDPZACALCULADA as float) < 1
							--	set @QTDPZACALCULADA = 1
							
							--Valida a quantidade mínima do plano zero para a peça
							if (CAST(@QTDPZACALCULADA as float)/CAST(@PeriodoPZ as float) < @QTD_MINIMA AND CAST(@QTDPZACALCULADA as float)/CAST(@PeriodoPZ as float) > Cast(0 as float))
								set @QT_PZ_PERIODO = @QTD_MINIMA
							else if (CAST(@QTDPZACALCULADA as float)/CAST(@PeriodoPZ as float) < @QTD_MINIMA AND CAST(@QTDPZACALCULADA as float)/CAST(@PeriodoPZ as float) = Cast(0 as float))
								set @QT_PZ_PERIODO = 0
							else
								Set @QT_PZ_PERIODO = ROUND(@QTDPZACALCULADA/@PeriodoPZ, 0)

							if CAST(@QTDPZACALCULADA as float) > 0
							begin
								print '----------------------------------------------------'
								print CAST(@QTD_MINIMA as float)
								print CAST(@QTDULTIMOANO as float)
								print CAST(@QTDPZACALCULADA as float)
								--print Cast(@QTDPZACALCULADAFLOAT as float)
								print CAST(@FATORPONDERACAO as float)
								print @MAQ_CART_TEC
								print @POT_CLIENTE
								print @CD_CLIENTE
								print @CD_PECA
								print '----------------------------------------------------'
							end

							--pega o estoque da peça do cliente E OBTEM A QTD A PEDIR subtraindo do qt_pz_periodo
							SELECT TOP 1
							@QTD_PEDIDO = @QT_PZ_PERIODO - ep.QT_PECA_ATUAL,
							@QTDPEDIDOPZ = @QT_PZ_PERIODO - ep.QT_PECA_ATUAL
							FROM tbEstoque e
							INNER JOIN TB_CLIENTE cli
							ON cli.CD_CLIENTE = e.CD_CLIENTE
							INNER JOIN tbEstoquePeca ep
							ON ep.ID_ESTOQUE = e.ID_ESTOQUE
							WHERE cli.CD_CLIENTE = @cd_cliente
							AND ep.CD_PECA = @CD_PECA
							AND e.FL_ATIVO = 'S'

							--Obtem quantidade em estoque por peça
							SELECT TOP 1
							@QT_ESTOQUECLI = ep.QT_PECA_ATUAL
							FROM tbEstoque e
							INNER JOIN TB_CLIENTE cli
							ON cli.CD_CLIENTE = e.CD_CLIENTE
							INNER JOIN tbEstoquePeca ep
							ON ep.ID_ESTOQUE = e.ID_ESTOQUE
							WHERE cli.CD_CLIENTE = @cd_cliente
							AND ep.CD_PECA = @CD_PECA
							AND e.FL_ATIVO = 'S'

							--Insere / atualiza o Plano Zero do Cliente
							if CAST(@QTDPZACALCULADA as float) > 0
							begin
							BEGIN TRANSACTION
								SET NOCOUNT ON
								UPDATE tbPlanoZeroClienteV2
								SET potencialPecas = potencialPecas + @POT_CLIENTE, qtdUltimoAno = @QTDULTIMOANO,
								qtdPZACalculada = Cast((Cast(@QTDULTIMOANO as float)/Cast(@POTENCIALPECA as float)) * (potencialPecas + @POT_CLIENTE) as float)
								WHERE codCliente = @CD_Cliente
								AND codPeca = @CD_PECA
								IF @@ROWCOUNT = 0
								BEGIN
								INSERT INTO tbPlanoZeroClienteV2 (codCliente, codPeca, potencialPecas, qtdPZACalculada, qtdEstoque, qtdPedidoPZ, qtdUltimoAno)
								VALUES (@CD_CLIENTE, @CD_PECA, @POT_CLIENTE, @QTDPZACALCULADA, @QT_ESTOQUECLI, @QTDPEDIDOPZ, @QTDULTIMOANO)
								END
							COMMIT TRANSACTION	
							end
						FETCH NEXT FROM @cursor2 INTO @CD_PECA, @POT_CLIENTE, @QTD_MINIMA, @CD_GRUPO_MODELO
					END
  				FETCH NEXT FROM @cursor INTO @CD_TECNICO,  @CD_CLIENTE, @PeriodoPZ
				END
	CLOSE @cursor
	DEALLOCATE @cursor

	
	 --pega os clientes que tem plano zero e cria os pedidos ------------------------------------------------------------------------------------------
  SET @cursor = CURSOR FOR

  SELECT DISTINCT
    tec.CD_TECNICO,
	cli.CD_Cliente
  FROM TB_TECNICO tec
  INNER JOIN TB_TECNICO_CLIENTE tcClie 
	ON tec.CD_TECNICO = tcClie.CD_TECNICO
	AND tcClie.CD_ORDEM = 1
  INNER JOIN TB_CLIENTE cli 
	ON cli.CD_CLIENTE = tcClie.CD_CLIENTE
  INNER JOIN tbPlanoZeroClienteV2 pzt
    ON pzt.codCliente = cli.CD_CLIENTE
  ORDER BY tec.CD_TECNICO

  OPEN @cursor
  FETCH NEXT FROM @cursor INTO @CD_TECNICO, @CD_CLIENTE
  WHILE @@FETCH_STATUS = 0

  
  BEGIN

   declare @count bigint;
   SELECT
      --@cd_peca = pzt.codPeca,
      --@CD_CLIENTE = pzt.codCliente,
	  @count = (select count (*) FROM tbPlanoZeroClienteV2 WHERE codCliente = @CD_CLIENTE)
    FROM tbPlanoZeroClienteV2 pzt
    WHERE pzt.codCliente = @CD_CLIENTE
    AND pzt.qtdPedidoPZ > 0
    ORDER BY pzt.codPeca

	--set @POTENCIALPECA = (select top 1 tpp.POTENCIALPECA from tbPotencialPecasV2 tpp where tpp.codPeca = @CD_PECA)
						

	--update tbPlanoZeroClienteV2
	--set qtdPZACalculada = Isnull((CAST(qtdUltimoAno as float)/CAST((select top 1 tpp.POTENCIALPECA from tbPotencialPecasV2 tpp where tpp.codPeca = codPeca ) as float)) * Cast(potencialPecas as float),0)

	SELECT 
	  pzt.codPeca,
      pzt.codCliente,
	  pzt.qtdPedidoPZ
	into #tempPecaPZ
      
	  --@count = (select count (*) FROM tbPlanoZeroClienteV2 WHERE codCliente = @CD_CLIENTE)
    FROM tbPlanoZeroClienteV2 pzt
    WHERE pzt.codCliente = @CD_CLIENTE
    AND pzt.qtdPedidoPZ > 0
	AND Cast(pzt.qtdPZACalculada as float) > 0
    ORDER BY pzt.codPeca

   --declare FirstCursor cursor for SELECT
   --   pzt.codPeca,
   --   pzt.codCliente,
	  --pzt.qtdPedidoPZ
	  ----@count = (select count (*) FROM tbPlanoZeroClienteV2 WHERE codCliente = @CD_CLIENTE)
   -- FROM tbPlanoZeroClienteV2 pzt
   -- WHERE pzt.codCliente = @CD_CLIENTE
   -- AND pzt.qtdPedidoPZ > 0
   -- ORDER BY pzt.codPeca
   --open FirstCursor 

   set @count = (select count(*) from #tempPecaPZ)

   if @count > 0
	begin
		SET NOCOUNT ON
		EXEC prcPedidoInsert @CD_TECNICO,       --@p_CD_TECNICO			 VARCHAR(6)		= NULL,
                         0,                 --@p_NR_DOCUMENTO		 NUMERIC(7,0)	= NULL,
                         @p_Data,           --@p_DT_CRIACAO			 DATETIME		= NULL,
                         NULL,              --@p_DT_ENVIO			 DATETIME		= NULL,
                         NULL,              --@p_DT_RECEBIMENTO		 DATETIME		= NULL,
                         NULL,              --@p_TX_OBS				 VARCHAR(255)	= NULL,
                         NULL,              --@p_PENDENTE			 VARCHAR(1)		= NULL,
                         NULL,              --@p_NR_DOC_ORI			 NUMERIC(18,0)	= NULL,
                         2,              --@p_ID_STATUS_PEDIDO	 BIGINT			= NULL,
                         'C',               --@p_TP_TIPO_PEDIDO		 CHAR(1)		= NULL,
                         @CD_CLIENTE,              --@p_CD_CLIENTE			 NUMERIC(6,0)	= NULL,
                         @ID_USUARIO,       --@p_nidUsuarioAtualizacaoNUMERIC(18,0)	= NULL,
                         NULL,              --@p_TOKEN    			 BIGINT			= NULL,
                         'N',               --@p_TP_Especial			 varchar(1)		= NULL,
                         NULL,              --@p_Responsavel			 varchar(70)    = null,
                         NULL,              --@p_Telefone			 varchar(12)	= null,
                         'W',              --@p_Origem				 varchar(1)		= null,
                         @p_TOKEN_GERADO,   --@p_TOKEN_GERADO    	 BIGINT		    OUTPUT,
                         @p_ID_PEDIDO       OUTPUT	
	
		SET NOCOUNT ON
		UPDATE TB_PEDIDO
			SET idControlePlanoZero = @idControlePlanoZero
			WHERE ID_PEDIDO = @p_ID_PEDIDO

		update TB_CLIENTE
			set DT_ULTIMOPLANOZERO = getdate()
		where CD_CLIENTE = @cd_cliente
         
		 while @count > 0
			begin

				select top 1 @CD_PECA = codPeca,
					   @QTDPEDIDOPZ = qtdPedidoPZ
				from #tempPecaPZ
				where codCliente = @cd_cliente

				set @valPeca = (select VL_PECA from TB_PECA where CD_PECA = @CD_PECA)

				SET NOCOUNT ON
				EXEC prcPedidoPecaInsert @p_ID_PEDIDO,
                               @CD_PECA,
                               @QTDPEDIDOPZ,
                               NULL,
                               NULL,
                               NULL,
                               NULL,
                               '6',
                               NULL,
                               NULL,
                               NULL,
                               NULL,
                               NULL,
                               NULL,
                               NULL,
                               @valPeca,
                               1,
                               NULL,
                               NULL,
                               NULL,
                               @p_TOKEN_GERADO,
                               @p_ID_ITEM


				set @CD_Pedido = (select CD_PEDIDO from TB_PEDIDO where ID_PEDIDO = @p_ID_PEDIDO)
				Select @POTENCIALPECA = tpp.POTENCIALPECA from tbPotencialPecasV2 tpp where tpp.codPeca = @CD_PECA
				set @QTDULTIMOANO = (select top 1 qtdUltimoAno from tbPlanoZeroClienteV2 where codPeca = @CD_PECA and codCliente = @cd_cliente)
				set @QTDPZACALCULADA = (select top 1 qtdPZACalculada from tbPlanoZeroClienteV2 where codPeca = @CD_PECA and codCliente = @cd_cliente)
				set @POT_CLIENTE = (select top 1 potencialPecas from tbPlanoZeroClienteV2 where codPeca = @CD_PECA and codCliente = @cd_cliente)

				SELECT TOP 1
						@QT_ESTOQUECLI = ep.QT_PECA_ATUAL
						FROM tbEstoque e
						INNER JOIN TB_CLIENTE cli
						ON cli.CD_CLIENTE = e.CD_CLIENTE
						INNER JOIN tbEstoquePeca ep
						ON ep.ID_ESTOQUE = e.ID_ESTOQUE
						WHERE cli.CD_CLIENTE = @cd_cliente
						AND ep.CD_PECA = @CD_PECA
						AND e.FL_ATIVO = 'S'
				
				SET NOCOUNT ON
				SET IDENTITY_INSERT tbLogPlanoZero ON
				insert into tbLogPlanoZero(idPlanoZero, tipoPlanoZero, codTecnicoCliente, codPeca, dtHoraCriacao, idUsuarioCriacao, potencialPecas, qtdUltimoano,
				qtdPZACalculada, potencialTotal, qtdPedidoPZ, qtdClientes, fatorPonderacao, nPedigoGerado, idPedido, qtdEstoque)
				values (@idControlePlanoZero, 'C', @cd_cliente, @CD_PECA, getdate(), @ID_USUARIO, @POT_CLIENTE, @QTDULTIMOANO,
				@QTDPZACALCULADA, @POTENCIALPECA, @QTDPEDIDOPZ, null, @FATORPONDERACAO, @CD_Pedido, @p_ID_PEDIDO, @QT_ESTOQUECLI)
				SET IDENTITY_INSERT tbLogPlanoZero OFF

				SET NOCOUNT ON
				delete 
				from #tempPecaPZ
				where codCliente = @cd_cliente and codPeca = @CD_PECA
			set @count = @count - 1
			  end
		   
	end
   
    drop table #tempPecaPZ
	

 --   select top 500 ped.DT_CRIACAO, pc.CD_PECA, pc.QTD_SOLICITADA from TB_PEDIDO ped
	--join TB_PEDIDO_PECA pc on pc.ID_PEDIDO = ped.ID_PEDIDO
	--where ped.TP_TIPO_PEDIDO = 'C' order by ped.id_pedido desc

	--delete from TB_PEDIDO where ID_PEDIDO > 19293 and TP_TIPO_PEDIDO = 'T'

	--delete from tbLogPlanoZero

	--select * from TB_PEDIDO where ID_PEDIDO > 19293 and TP_TIPO_PEDIDO = 'T'
	--update TB_PECA set QTD_PlanoZero = 6

    
	
	--passa o numero do Controle
    --UPDATE TB_PEDIDO
    --SET idControlePlanoZero = @idControlePlanoZero
    --WHERE ID_PEDIDO = @p_ID_PEDIDO

--pega as peças do plano zero do cliente e cria os itens do pedido-----------------------------------------------------------------------------------
    
    FETCH NEXT FROM @cursor INTO @CD_TECNICO, @CD_CLIENTE
  END
  CLOSE @cursor
  DEALLOCATE @cursor

  --Update tbControlePlanoZero
  --set statusPlanoZero = 'P'
  --where idPlanoZero = @idControlePlanoZero

	--SELECT * FROM tbPlanoZero WHERE  CD_PECA = 'HB004117113'
	--SELECT * FROM TB_PECA WHERE CD_PECA = 'HB004117113'
	--SELECT * FROM tbEstoquePeca WHERE CD_PECA = 'HB004117113'
	--Select * from tbPlanoZeroTecnico order by codTecnico, codPeca
	--Select * from tbPlanoZeroClienteV2 where codcliente = 39248 where qtdPedidoPZ is not null order by codPeca
	--Select * from tbPotencialPecasV2

	end --remover

	--select * from tbControlePlanoZero

/*
	END TRY

	BEGIN CATCH

			SELECT	@cdsErrorMessage	= ERROR_MESSAGE(),
					@nidErrorSeverity	= ERROR_SEVERITY(),
					@nidErrorState		= ERROR_STATE();

			RAISERROR (@cdsErrorMessage, -- Message text.
					   @nidErrorSeverity, -- Severity.
					   @nidErrorState -- State.
					   )
	END CATCH
END
*/

 
/* 
			SELECT * -- COUNT(*)
			FROM tbGrupoModelo gm
			INNER JOIN TB_MODELO md ON md.CD_GRUPO_MODELO = gm.cd_grupoModelo
			INNER JOIN tbPlanoZero pz ON pz.CD_GRUPO_MODELO = md.CD_GRUPO_MODELO
			INNER JOIN TB_PECA pc ON pc.CD_PECA = pz.CD_PECA
			WHERE pz.CD_CRITICIDADE_ABC = 'A'
			AND pc.CD_PECA = 'H0002215400'
			AND md.CD_GRUPO_MODELO = '120AF'

 		SELECT *, pz.QT_PECA_MODELO, MD.CD_GRUPO_MODELO
		  --COUNT(*), MAX(pz.QT_PECA_MODELO)
		FROM TB_ATIVO_CLIENTE AC
		INNER JOIN TB_ATIVO_FIXO af ON af.CD_ATIVO_FIXO = ac.CD_ATIVO_FIXO
		INNER JOIN TB_MODELO md ON md.CD_MODELO = af.CD_MODELO
		INNER JOIN tbPlanoZero pz ON pz.CD_GRUPO_MODELO = md.CD_GRUPO_MODELO
		INNER JOIN TB_PECA pc ON pc.CD_PECA = pz.CD_PECA
		WHERE ac.DT_DEVOLUCAO IS NULL AND pz.CD_CRITICIDADE_ABC = 'A'

		AND md.CD_GRUPO_MODELO = '120AF'
		 AND pc.CD_PECA = 'H0002215400'
		 
		 SELECT * FROM tbPotencialPecasV2 TPP ORDER BY TPP.codPeca 
*/


--update tbEstoquePeca
--set QT_PECA_ATUAL = 5

--select * from TB_PECA where QTD_PlanoZero > 0
end