GO
/****** Object:  StoredProcedure [dbo].[prcPlanoZeroGerarPedidos]    Script Date: 04/03/2024 11:19:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Use COMODATODEV
-- exec prcPlanoZeroGerarPedidos 15434
ALTER Procedure [dbo].[prcPlanoZeroGerarPedidos]
	@ID_USUARIO int,
	@idControlePlanoZero int
AS
BEGIN
	--Delete from tbPlanoZeroCliente
	Delete from tbPlanoZeroTecnico
	Delete from tbPotencialPecasV2

  DECLARE 
    @cursor CURSOR,						      @FATORPONDERACAO int,
    @cursor2 CURSOR,					      @POTENCIALPECA int,
    @FirstCursor CURSOR,
	@QTDPEDIDOPZ int,
	@QTDPZACALCULADA Float = 0,			      @QTDULTIMOANO int = 0,
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
	@QT_ESTOQUECLI int,						  @idEstoque INT,
	@Tel varchar(20),						  @NMPed varchar(50),
	@valPeca numeric(14,2),				      @CD_Pedido bigint,
	@POT_CLIENTE int,						  @PeriodoPZ int
	Begin 
		
	--Pega as peças do Plano Zero de Criticidade 'A'  e qtd por maq
	SET @cursor = CURSOR FOR
	SELECT	pz.CD_PECA, PZ.QT_PECA_MODELO, PZ.CD_GRUPO_MODELO
	FROM tbPlanoZero pz
	WHERE pz.CD_CRITICIDADE_ABC = 'A'
	--AND MONTH(pz.DT_CRIACAO) = MONTH(GETDATE())
	--AND YEAR(pz.DT_CRIACAO) = YEAR(GETDATE())
	OPEN @cursor
		FETCH NEXT FROM @cursor INTO @CD_PECA, @QT_PECA_MODELO, @CD_GRUPO_MODELO
			  WHILE @@FETCH_STATUS = 0
		BEGIN
			--Verifica a quantidade de maquinas EM CARTEIRA 
			SELECT
			@MAQ_CART = COUNT(*) FROM TB_ATIVO_CLIENTE (nolock)
			INNER JOIN TB_ATIVO_FIXO (nolock) ON TB_ATIVO_CLIENTE.CD_ATIVO_FIXO = TB_ATIVO_FIXO.CD_ATIVO_FIXO
			LEFT JOIN TB_MODELO ON TB_ATIVO_FIXO.CD_MODELO = TB_MODELO.CD_MODELO
			LEFT JOIN tbGrupoModelo on TB_MODELO.CD_GRUPO_MODELO = tbGrupoModelo.cd_grupoModelo
			Where DT_DEVOLUCAO IS NULL
			AND (tbGrupoModelo.cd_grupoModelo = @CD_GRUPO_MODELO)
			AND FL_STATUS=1

			--Cria o Potencial Total de Peças
			BEGIN TRANSACTION
				UPDATE tbPotencialPecasV2
					SET potencialPeca = potencialPeca + (@MAQ_CART * @QT_PECA_MODELO)
					WHERE codPeca = @CD_PECA
				IF @@ROWCOUNT = 0
				BEGIN
					INSERT INTO tbPotencialPecasV2 (codPeca, potencialPeca)
					VALUES (@CD_PECA, @MAQ_CART * @QT_PECA_MODELO)
				END
			COMMIT TRANSACTION

		FETCH NEXT FROM @cursor INTO @CD_PECA, @QT_PECA_MODELO, @CD_GRUPO_MODELO
		END
	CLOSE @cursor
	DEALLOCATE @cursor
	

	--pega a lista de técnicos que estão ativos e tem cliente sem plano zero 
	SET @cursor = CURSOR FOR
	SELECT distinct
	tt.CD_TECNICO --, cl.CD_CLIENTE
	FROM TB_TECNICO tt
	INNER JOIN TB_TECNICO_CLIENTE tc
	ON tc.CD_TECNICO = tt.CD_TECNICO
	INNER JOIN TB_CLIENTE cl
	ON cl.CD_CLIENTE = tc.CD_CLIENTE
	WHERE tt.FL_ATIVO = 'S'
	AND tc.CD_ORDEM = 1
	AND (cl.FL_AtivaPlanoZero = 'N'
	OR cl.FL_AtivaPlanoZero IS NULL)
	order by tt.CD_TECNICO

	OPEN @cursor
		FETCH NEXT FROM @cursor INTO @CD_TECNICO --, @CD_CLIENTE
			  WHILE @@FETCH_STATUS = 0
				BEGIN
					Set @cursor2 = CURSOR FOR
						SELECT DISTINCT PC.CD_PECA, pz.QT_PECA_MODELO, pc.QTD_PlanoZero,pz.CD_GRUPO_MODELO--pz.QT_ESTOQUE_MINIMO
						FROM TB_TECNICO tt
						INNER JOIN TB_TECNICO_CLIENTE tc ON tc.CD_TECNICO = tt.CD_TECNICO
						INNER JOIN TB_CLIENTE cl ON cl.CD_CLIENTE = tc.CD_CLIENTE
						INNER JOIN TB_ATIVO_CLIENTE ac ON ac.CD_CLIENTE = cl.CD_CLIENTE
						INNER JOIN TB_ATIVO_FIXO af ON af.CD_ATIVO_FIXO = ac.CD_ATIVO_FIXO
						INNER JOIN TB_MODELO md ON md.CD_MODELO = af.CD_MODELO
						INNER JOIN tbPlanoZero pz ON pz.CD_GRUPO_MODELO = md.CD_GRUPO_MODELO
						INNER JOIN TB_PECA pc ON pc.CD_PECA = pz.CD_PECA
						WHERE tc.CD_ORDEM = 1
						AND tt.FL_ATIVO = 'S' AND cl.DT_DESATIVACAO IS NULL
						AND ac.DT_DEVOLUCAO IS NULL AND pz.CD_CRITICIDADE_ABC = 'A'
						AND (cl.FL_AtivaPlanoZero = 'N' OR cl.FL_AtivaPlanoZero IS NULL)
						--AND MONTH(pz.DT_CRIACAO) = MONTH(GETDATE())
						--AND YEAR(pz.DT_CRIACAO) = YEAR(GETDATE()) 
						AND TC.CD_TECNICO = @CD_TECNICO
						--and cl.CD_CLIENTE = @cd_cliente 
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
						AND (cl.FL_AtivaPlanoZero = 'N'
						OR cl.FL_AtivaPlanoZero IS NULL)
						
						--pega o fator de ponderação do tecnico
						Select top(1) @FATORPONDERACAO = fator 
						from TB_PONDERACAO_pz where MIN_CLIENTES <= @QT_CLIENTES and MAX_CLIENTES >= @QT_CLIENTES

						--pega a quantidade do ultimo ano
						SELECT 
							@QTDULTIMOANO = IsNull(Sum(tpc.QTD_RECEBIDA), 0)
						FROM
							TB_PEDIDO tp
							INNER JOIN TB_PEDIDO_PECA tpc
							ON tp.ID_PEDIDO = tpc.ID_PEDIDO
							INNER JOIN TB_PECA pc
							ON tpc.CD_PECA = pc.CD_PECA
							
						WHERE
							(tp.DT_CRIACAO) > (DATEADD(day, -364, GETDATE()))
							and tp.DT_CRIACAO < (DATEADD(day, -1, GETDATE()))
							AND (tp.CD_TECNICO		= @CD_TECNICO)
							and tp.ID_STATUS_PEDIDO = 4
							and tpc.QTD_RECEBIDA is not null
							and tpc.CD_PECA = @CD_PECA
							and (tp.tp_tipo_pedido = 'A' or tp.tp_tipo_pedido = 'T')

						--Pega o Potencial da Peça
						Select @POTENCIALPECA = tpp.POTENCIALPECA from tbPotencialPecasV2 tpp where tpp.codPeca = @CD_PECA

						--Paga a qtd de maq em carteira do tecnico
						SELECT
						@MAQ_CART_TEC = COUNT(*)
						FROM TB_ATIVO_CLIENTE ac
						INNER JOIN TB_TECNICO_CLIENTE tc ON tc.CD_CLIENTE = ac.CD_CLIENTE
						INNER JOIN TB_ATIVO_FIXO af ON af.CD_ATIVO_FIXO = ac.CD_ATIVO_FIXO
						INNER JOIN TB_MODELO md ON md.CD_MODELO = af.CD_MODELO
						INNER JOIN tbPlanoZero pz ON pz.CD_GRUPO_MODELO = md.CD_GRUPO_MODELO
						INNER JOIN TB_PECA pc ON pc.CD_PECA = pz.CD_PECA
						WHERE 
						ac.DT_DEVOLUCAO IS NULL AND pz.CD_CRITICIDADE_ABC = 'A'
						AND md.CD_GRUPO_MODELO = @CD_GRUPO_MODELO
						and pc.CD_PECA = @CD_PECA
						--and tc.CD_CLIENTE = @CD_CLIENTE
						and tc.CD_TECNICO = @CD_TECNICO 
						
							--Calcula o Plano Zero Anual
							SET @QTDPZACALCULADA = (Cast(@QTDULTIMOANO as Float)/@POTENCIALPECA) * @MAQ_CART_TEC * (1 + @FATORPONDERACAO)

							
							--Valida a quantidade mínima do plano zero para a peça
							if Cast(@QTDPZACALCULADA as float)/12 < @QTD_MINIMA and @QTDPZACALCULADA > 0
								set @QT_PZ_PERIODO = @QTD_MINIMA
							else if Cast(@QTDPZACALCULADA as float)/12 < @QTD_MINIMA and @QTDPZACALCULADA = 0
								set @QT_PZ_PERIODO = 0
							else
								Set @QT_PZ_PERIODO = ROUND(@QTDPZACALCULADA/12, 0)

							

							
							if @QTDULTIMOANO > 0
							begin
								print '----------------------------------------------------'
								print CAST(@QTD_MINIMA as float)
								print CAST(@QTDULTIMOANO as float)
								print CAST(@QTDPZACALCULADA as float)
								--print Cast(@QTDPZACALCULADAFLOAT as float)
								print CAST(@FATORPONDERACAO as float)
								print @MAQ_CART_TEC
								print @POT_CLIENTE
								print @CD_TECNICO
								print @CD_PECA
								print '----------------------------------------------------'
							end

							--pega o estoque da peça do tecnico E OBTEM A QTD A PEDIR subtraindo do qt_pz_periodo
							SELECT TOP 1
							@QTD_PEDIDO = @QT_PZ_PERIODO - ep.QT_PECA_ATUAL,
							@QTDPEDIDOPZ = @QT_PZ_PERIODO - ep.QT_PECA_ATUAL,
							@idEstoque = e.ID_ESTOQUE
							FROM tbEstoque e
							INNER JOIN TB_TECNICO tec
							ON tec.CD_TECNICO = e.CD_TECNICO
							INNER JOIN tbEstoquePeca ep
							ON ep.ID_ESTOQUE = e.ID_ESTOQUE
							WHERE 
							--tec.CD_TECNICO = @CD_TECNICO
							e.CD_TECNICO = @CD_TECNICO
							AND ep.CD_PECA = @CD_PECA
							AND e.TP_ESTOQUE_TEC_3M = 'TEC'
							--AND tec.ID_USUARIO = e.ID_USU_RESPONSAVEL
							AND e.FL_ATIVO = 'S'

							SELECT TOP 1
							@QT_ESTOQUECLI = ep.QT_PECA_ATUAL
							FROM tbEstoque e
							INNER JOIN TB_TECNICO cli
							ON cli.CD_TECNICO = e.CD_TECNICO
							INNER JOIN tbEstoquePeca ep
							ON ep.ID_ESTOQUE = e.ID_ESTOQUE
							WHERE cli.CD_TECNICO = @CD_TECNICO
							AND ep.CD_PECA = @CD_PECA
							AND e.FL_ATIVO = 'S'

							--Insere / atualiza o Plano Zero do Tecnico
							BEGIN TRANSACTION
								UPDATE tbPlanoZeroTecnico
								SET potencialPecas = potencialPecas + @POTENCIALPECA, qtdUltimoAno = @QTDULTIMOANO, qtdPedidoPZ = @QTDPEDIDOPZ
								WHERE codTecnico = @CD_TECNICO
								AND codPeca = @CD_PECA
								IF @@ROWCOUNT = 0
								BEGIN
								INSERT INTO tbPlanoZeroTecnico (codTecnico, codPeca, qtdClientes, fatorPonderacao, potencialPecas, qtdPZACalculada, qtdPedidoPZ, qtdUltimoAno, qtdEstoque)
								VALUES (@CD_TECNICO, @CD_PECA, @QT_CLIENTES, @FATORPONDERACAO, @POT_CLIENTE, @QTDPZACALCULADA, @QT_PZ_PERIODO, @QTDULTIMOANO, @QT_ESTOQUECLI)
								END
							COMMIT TRANSACTION	
						FETCH NEXT FROM @cursor2 INTO @CD_PECA, @POT_CLIENTE, @QTD_MINIMA, @CD_GRUPO_MODELO
					END
  				FETCH NEXT FROM @cursor INTO @CD_TECNICO --,  @CD_CLIENTE
				END
	CLOSE @cursor
	DEALLOCATE @cursor

	--  --Cria o Controle Plano Zero
 --   INSERT INTO tbControlePlanoZero (dtHoraCriacao, idUsuarioCriacao, dtHoraCancelamento, idUsuarioCancelamento, statusPlanoZero, mensagem)
 --   VALUES (GETDATE(), @ID_USUARIO, NULL, NULL, 'A', 'Processamento Plano Zero Iniciado')
	--Set @idControlePlanoZero = @@IDENTITY
	
  
  --pega os tecnicos que tem plano zero e cria os pedidos ------------------------------------------------------------------------------------------
  SET @cursor = CURSOR FOR
	  SELECT DISTINCT
		tec.CD_TECNICO
	  FROM TB_TECNICO tec
	  INNER JOIN tbplanozerotecnico pzt
		ON pzt.codTecnico = tec.CD_TECNICO
	  ORDER BY tec.CD_TECNICO

		  OPEN @cursor
		  FETCH NEXT FROM @cursor INTO @CD_TECNICO
		  WHILE @@FETCH_STATUS = 0
		  BEGIN

		  declare @count bigint;

		   SELECT pzt.codPeca,
			  pzt.codTecnico,
			  pzt.qtdPedidoPZ
			  into #temppecaTecnico
			  --@cd_peca = pzt.codPeca,
			  --@CD_CLIENTE = pzt.codCliente,
			  --@count = (select count (*) FROM tbPlanoZeroTecnico WHERE codTecnico = @CD_TECNICO)
			FROM tbPlanoZeroTecnico pzt
			WHERE pzt.codTecnico = @CD_TECNICO
			AND pzt.qtdPedidoPZ > 0
			AND Cast(pzt.qtdPZACalculada as float) > 0
			ORDER BY pzt.codPeca

			set @count = (select count(*) from #temppecaTecnico)
			set @NMPed = (select NM_TECNICO from TB_TECNICO where CD_TECNICO = @CD_TECNICO)
			set @Tel = (select TX_TELEFONE from TB_TECNICO where CD_TECNICO = @CD_TECNICO)

			if @count > 0	
			begin
				exec prcPedidoInsert @CD_TECNICO,       --@p_CD_TECNICO			 VARCHAR(6)		= NULL,
								 0,                 --@p_NR_DOCUMENTO		 NUMERIC(7,0)	= NULL,
								 @p_Data,           --@p_DT_CRIACAO			 DATETIME		= NULL,
								 NULL,              --@p_DT_ENVIO			 DATETIME		= NULL,
								 NULL,              --@p_DT_RECEBIMENTO		 DATETIME		= NULL,
								 NULL,              --@p_TX_OBS				 VARCHAR(255)	= NULL,
								 NULL,              --@p_PENDENTE			 VARCHAR(1)		= NULL,
								 NULL,              --@p_NR_DOC_ORI			 NUMERIC(18,0)	= NULL,
								 2,              --@p_ID_STATUS_PEDIDO	 BIGINT			= NULL,
								 'A',               --@p_TP_TIPO_PEDIDO		 CHAR(1)		= NULL,
								 NULL,              --@p_CD_CLIENTE			 NUMERIC(6,0)	= NULL,
								 @ID_USUARIO,       --@p_nidUsuarioAtualizacaoNUMERIC(18,0)	= NULL,
								 NULL,              --@p_TOKEN    			 BIGINT			= NULL,
								 'N',               --@p_TP_Especial		 varchar(1)		= NULL,
								 @NMPed,              --@p_Responsavel		 varchar(70)    = null,
								 @Tel,              --@p_Telefone			 varchar(12)	= null,
								 'W',              --@p_Origem				 varchar(1)		= null,
								 @p_TOKEN_GERADO,   --@p_TOKEN_GERADO    	 BIGINT		    OUTPUT,
								 @p_ID_PEDIDO output       --@p_ID_PEDIDO			 NUMERIC(9,0)	OUTPUT					

			--fetch FirstCursor into @cd_peca, @CD_TECNICO, @QTDPEDIDOPZ
			--passa o numero do Controle
			UPDATE TB_PEDIDO
			SET idControlePlanoZero = @idControlePlanoZero
			WHERE ID_PEDIDO = @p_ID_PEDIDO

			while @count > 0
			begin

				select top 1 @CD_PECA = codPeca,
					   @QTDPEDIDOPZ = qtdPedidoPZ
				from #temppecaTecnico
				where codTecnico = @CD_TECNICO

			

	--pega as peças do plano zero do técnico e cria os itens do pedido-----------------------------------------------------------------------------------
				

				set @valPeca = (select VL_PECA from TB_PECA where CD_PECA = @CD_PECA)

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
				
--select * from tbLogPlanoZero

				SELECT TOP 1
							@QT_ESTOQUECLI = ep.QT_PECA_ATUAL
							FROM tbEstoque e
							INNER JOIN TB_TECNICO cli
							ON cli.CD_TECNICO = e.CD_TECNICO
							INNER JOIN tbEstoquePeca ep
							ON ep.ID_ESTOQUE = e.ID_ESTOQUE
							WHERE cli.CD_TECNICO = @CD_TECNICO
							AND ep.CD_PECA = @CD_PECA
							AND e.FL_ATIVO = 'S'

				set @CD_Pedido = (select CD_PEDIDO from TB_PEDIDO where ID_PEDIDO = @p_ID_PEDIDO)
				Select @POTENCIALPECA = tpp.POTENCIALPECA from tbPotencialPecasV2 tpp where tpp.codPeca = @CD_PECA
				
				set @QTDULTIMOANO = (select top 1 qtdUltimoAno from tbPlanoZeroTecnico where codPeca = @CD_PECA and codTecnico = @CD_TECNICO)
				set @QTDPZACALCULADA = (select top 1 qtdPZACalculada from tbPlanoZeroTecnico where codPeca = @CD_PECA and codTecnico = @CD_TECNICO)
				
				if Cast(@QTDPZACALCULADA as float) > 0
				begin
					SET IDENTITY_INSERT tbLogPlanoZero ON
					insert into tbLogPlanoZero(idPlanoZero, tipoPlanoZero, codTecnicoCliente, codPeca, dtHoraCriacao, idUsuarioCriacao, potencialPecas, qtdUltimoano,
					qtdPZACalculada, potencialTotal, qtdPedidoPZ, qtdClientes, fatorPonderacao, nPedigoGerado, idPedido, qtdEstoque)
					values (@idControlePlanoZero, 'T', @CD_TECNICO, @CD_PECA, getdate(), @ID_USUARIO, @POT_CLIENTE, @QTDULTIMOANO,
					@QTDPZACALCULADA, @POTENCIALPECA, @QTDPEDIDOPZ, null, @FATORPONDERACAO, @CD_Pedido, @p_ID_PEDIDO, @QT_ESTOQUECLI)
					SET IDENTITY_INSERT tbLogPlanoZero OFF
				end
				delete 
				from #temppecaTecnico
				where codTecnico = @CD_TECNICO and codPeca = @CD_PECA

				set @count = @count - 1
			  end

		end
		drop table #temppecaTecnico
		--  FETCH NEXT FROM @cursor2 INTO @cd_peca, @QTDPEDIDOPZ, @POTENCIALPECA, @QTDULTIMOANO, @QTDPZACALCULADA
		--END
		--CLOSE @cursor2
		--DEALLOCATE @cursor2
		FETCH NEXT FROM @cursor INTO @CD_TECNICO
	  END
	  CLOSE @cursor
	  DEALLOCATE @cursor
 
	  --Update tbControlePlanoZero
	  --set statusPlanoZero = 'P'
	  --where idPlanoZero = @idControlePlanoZero

	 DELETE w
		FROM TB_PEDIDO_PECA w
		JOIN TB_PEDIDO e
		  ON e.ID_PEDIDO = w.ID_PEDIDO
		WHERE (w.ST_STATUS_ITEM = '1' AND e.idControlePlanoZero is not null)
			  OR(w.QTD_SOLICITADA = 0 AND e.idControlePlanoZero is not null)

	--select * from tbControleplanozero

	--UPDATE tb_pedido_peca SET st_status_item = '4' FROM tb_pedido_peca A JOIN tb_pedido B ON A.id_pedido = B.id_pedido WHERE b.idcontroleplanozero = 113
	--update tb_pedido set id_status_pedido = 7 where idControlePlanoZero = 113
	--update tbControlePlanoZero set statusPlanoZero = 'C' where idPlanoZero = 113
	--select * from tb_pedido where idControlePlanozero = 135

	--select top 10 * from tb_pedido order by id_pedido desc

	--select w.*
	--	FROM TB_PEDIDO_PECA w
	--	JOIN TB_PEDIDO e
	--	  ON e.ID_PEDIDO = w.ID_PEDIDO
	--	WHERE (w.ST_STATUS_ITEM = '1' AND e.idControlePlanoZero is not null)
	--		  OR(w.QTD_SOLICITADA = 0 AND e.idControlePlanoZero is not null)
	--	order by e.ID_pedido

	END 

	--BEGIN CATCH

	--		SELECT	@cdsErrorMessage	= ERROR_MESSAGE(),
	--				@nidErrorSeverity	= ERROR_SEVERITY(),
	--				@nidErrorState		= ERROR_STATE();

	--		RAISERROR (@cdsErrorMessage, -- Message text.
	--				   @nidErrorSeverity, -- Severity.
	--				   @nidErrorState -- State.
	--				   )
	--END CATCH
END


--   select top 500 ped.DT_CRIACAO, pc.CD_PECA, pc.QTD_SOLICITADA from TB_PEDIDO ped
	--join TB_PEDIDO_PECA pc on pc.ID_PEDIDO = ped.ID_PEDIDO
	--where ped.TP_TIPO_PEDIDO = 'T' order by ped.id_pedido desc

	--delete from TB_PEDIDO where ID_PEDIDO > 19293 and TP_TIPO_PEDIDO = 'T'
	--delete from tbLogPlanoZero

	--select * from TB_PEDIDO where ID_PEDIDO > 19293 and TP_TIPO_PEDIDO = 'T'
	--update TB_PECA set QTD_PlanoZero = 6

	--select * from tbPlanoZeroTecnico where codTecnico = '347810'
	--select * from TB_PEDIDO_PECA where ID_PEDIDO = 20749 and st_status_item = '6'
	--select * from TB_PEDIDO_PECA where ID_PEDIDO = 20749 and st_status_item = '1'
	--select top 10 * from TB_PEDIDO_PECA where vl_peca is not null order by id_pedido desc
	--select * from tb_pedido where CD_PEDIDO = 9414

--	delete from TB_PEDIDO_PECA where id_Pedido > 19293 st_status_item = '1' and ID_PEDIDO in (20746
--,20747
--,20748
--,20749
--,20750
--,20751
--,20752
--,20753
--,20754
--,20755
--,20756
--,20757
--,20758
--,20759
--,20760
--,20761
--,20762
--,20763
--,20764
--,20765
--,20766
--,20767
--,20768
--,20769
--,20770
--,20771
--,20772
--,20773
--,20774
--,20775
--,20776
--,20777)
 