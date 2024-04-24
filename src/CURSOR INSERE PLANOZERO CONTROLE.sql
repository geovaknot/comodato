-- Use COMODATODEV
-- exec prcPlanoZeroGerarPedidos 15434
Alter Procedure prcPlanoZeroGerarPedidos
@ID_USUARIO int --15434
AS
BEGIN
	Delete from tbPlanoZeroCliente
	Delete from tbPlanoZeroTecnico
	Delete from tbPotencialPecas

  DECLARE 
    @cursor CURSOR,						      @FATORPONDERACAO int,
    @cursor2 CURSOR,					      @POTENCIALPECA int,
    @idControlePlanoZero int = 0,		      @QTDPEDIDOPZ int,
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
	@QT_ESTOQUECLI int
	Begin Try
		
	--Pega as peças do Plano Zero de Criticidade 'A'  e qtd por maq
	SET @cursor = CURSOR FOR
	SELECT	pz.CD_PECA, PZ.QT_PECA_MODELO, PZ.CD_GRUPO_MODELO
	FROM tbPlanoZero pz
	WHERE pz.CD_CRITICIDADE_ABC = 'A'
	AND MONTH(pz.DT_CRIACAO) = MONTH(GETDATE())
	AND YEAR(pz.DT_CRIACAO) = YEAR(GETDATE())
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
				UPDATE tbPotencialPecas
					SET potencialPeca = potencialPeca + (@MAQ_CART * @QT_PECA_MODELO)
					WHERE codPeca = @CD_PECA
				IF @@ROWCOUNT = 0
				BEGIN
					INSERT INTO tbPotencialPecas (codPeca, potencialPeca)
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
						SELECT DISTINCT PC.CD_PECA, pz.QT_PECA_MODELO, pz.QT_ESTOQUE_MINIMO
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
						AND MONTH(pz.DT_CRIACAO) = MONTH(GETDATE())
						AND YEAR(pz.DT_CRIACAO) = YEAR(GETDATE()) 
						AND TC.CD_TECNICO = @CD_TECNICO
						--and cl.CD_CLIENTE = @cd_cliente 
						ORDER BY PC.CD_PECA
					OPEN @cursor2
					FETCH NEXT FROM @cursor2 INTO @CD_PECA, @POTENCIALPECA, @QTD_MINIMA
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
							INNER JOIN tbStatusPedido sp
							ON tp.ID_STATUS_PEDIDO = sp.ID_STATUS_PEDIDO
						WHERE
							YEAR(tp.DT_CRIACAO) = YEAR(DATEADD(YEAR, -1, GETDATE()))
							AND (tp.CD_TECNICO		= @CD_TECNICO)
							and sp.ID_STATUS_PEDIDO = 4
							and tpc.QTD_RECEBIDA is not null
							and tpc.CD_PECA = @CD_PECA

						--Pega o Potencial da Peça
						Select @POTENCIALPECA = tpp.POTENCIALPECA from tbPotencialPecas tpp where tpp.codPeca = @CD_PECA

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
						print 'MAQ_CART_TEC '
						print @MAQ_CART_TEC

							--Calcula o Plano Zero Anual
							SET @QTDPZACALCULADA = (Cast(@QTDULTIMOANO as Float)/@POTENCIALPECA) * @MAQ_CART_TEC * (1 + @FATORPONDERACAO)

							--Valida a quantidade mínima do plano zero para a peça
							if @QTDPZACALCULADA/12 < @QTD_MINIMA
								set @QT_PZ_PERIODO = @QTD_MINIMA
							else
								Set @QT_PZ_PERIODO = ROUND(@QTDPZACALCULADA/12, 0)

							--pega o estoque da peça do tecnico E OBTEM A QTD A PEDIR subtraindo do qt_pz_periodo
							SELECT TOP 1
							@QTD_PEDIDO = @QT_PZ_PERIODO - ep.QT_PECA_ATUAL
							FROM tbEstoque e
							INNER JOIN TB_TECNICO tec
							ON tec.CD_TECNICO = e.CD_TECNICO
							INNER JOIN tbEstoquePeca ep
							ON ep.ID_ESTOQUE = e.ID_ESTOQUE
							WHERE tec.CD_TECNICO = @CD_TECNICO
							AND ep.CD_PECA = @CD_PECA
							AND tec.ID_USUARIO = e.ID_USU_RESPONSAVEL
							AND e.FL_ATIVO = 'S'

							--Insere / atualiza o Plano Zero do Tecnico
							BEGIN TRANSACTION
								UPDATE tbPlanoZeroTecnico
								SET potencialPecas = potencialPecas + @POTENCIALPECA, qtdUltimoAno = @QTDULTIMOANO
								WHERE codTecnico = @CD_TECNICO
								AND codPeca = @CD_PECA
								IF @@ROWCOUNT = 0
								BEGIN
								INSERT INTO tbPlanoZeroTecnico (codTecnico, codPeca, qtdClientes, fatorPonderacao, potencialPecas, qtdPZACalculada, qtdPedidoPZ, qtdUltimoAno)
								VALUES (@CD_TECNICO, @CD_PECA, @QT_CLIENTES, @FATORPONDERACAO, @POTENCIALPECA, @QTDPZACALCULADA, @QT_PZ_PERIODO, @QTDULTIMOANO)
								END
							COMMIT TRANSACTION	
						FETCH NEXT FROM @cursor2 INTO @CD_PECA, @POTENCIALPECA, @QTD_MINIMA
					END
  				FETCH NEXT FROM @cursor INTO @CD_TECNICO --,  @CD_CLIENTE
				END
	CLOSE @cursor
	DEALLOCATE @cursor

	  --Cria o Controle Plano Zero
    INSERT INTO tbControlePlanoZero (dtHoraCriacao, idUsuarioCriacao, dtHoraCancelamento, idUsuarioCancelamento, statusPlanoZero, mensagem)
    VALUES (GETDATE(), @ID_USUARIO, NULL, NULL, 'A', 'Processamento Plano Zero Iniciado')
	Set @idControlePlanoZero = @@IDENTITY
	Print 'idControlePlanoZero'
	print @idControlePlanoZero
  
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

		   EXEC prcPedidoInsert @CD_TECNICO,       --@p_CD_TECNICO			 VARCHAR(6)		= NULL,
								 0,                 --@p_NR_DOCUMENTO		 NUMERIC(7,0)	= NULL,
								 @p_Data,           --@p_DT_CRIACAO			 DATETIME		= NULL,
								 NULL,              --@p_DT_ENVIO			 DATETIME		= NULL,
								 NULL,              --@p_DT_RECEBIMENTO		 DATETIME		= NULL,
								 NULL,              --@p_TX_OBS				 VARCHAR(255)	= NULL,
								 NULL,              --@p_PENDENTE			 VARCHAR(1)		= NULL,
								 NULL,              --@p_NR_DOC_ORI			 NUMERIC(18,0)	= NULL,
								 NULL,              --@p_ID_STATUS_PEDIDO	 BIGINT			= NULL,
								 'T',               --@p_TP_TIPO_PEDIDO		 CHAR(1)		= NULL,
								 NULL,              --@p_CD_CLIENTE			 NUMERIC(6,0)	= NULL,
								 @ID_USUARIO,       --@p_nidUsuarioAtualizacaoNUMERIC(18,0)	= NULL,
								 NULL,              --@p_TOKEN    			 BIGINT			= NULL,
								 'N',               --@p_TP_Especial		 varchar(1)		= NULL,
								 NULL,              --@p_Responsavel		 varchar(70)    = null,
								 NULL,              --@p_Telefone			 varchar(12)	= null,
								 NULL,              --@p_Origem				 varchar(1)		= null,
								 @p_TOKEN_GERADO,   --@p_TOKEN_GERADO    	 BIGINT		    OUTPUT,
								 @p_ID_PEDIDO output       --@p_ID_PEDIDO			 NUMERIC(9,0)	OUTPUT					

			--passa o numero do Controle
			UPDATE TB_PEDIDO
			SET idControlePlanoZero = @idControlePlanoZero
			WHERE ID_PEDIDO = @p_ID_PEDIDO

			print 'p_ID_PEDIDO'
			print @p_ID_PEDIDO

	--pega as peças do plano zero do técnico e cria os itens do pedido-----------------------------------------------------------------------------------
		SET @cursor2 = CURSOR FOR
    
		SELECT
		  pzt.codPeca, pzt.qtdPedidoPZ, pzt.potencialPecas, pzt.qtdUltimoAno, pzt.qtdPZACalculada
		FROM tbPlanoZeroTecnico pzt
		WHERE pzt.codTecnico = @CD_TECNICO
		AND pzt.qtdPedidoPZ > 0
		ORDER BY pzt.codPeca

		OPEN @cursor2
		FETCH NEXT FROM @cursor2 INTO @cd_peca, @QTDPEDIDOPZ, @POTENCIALPECA, @QTDULTIMOANO, @QTDPZACALCULADA
		WHILE @@FETCH_STATUS = 0
		BEGIN
		  EXEC prcPedidoPecaInsert @p_ID_PEDIDO,
								   @CD_PECA,
								   @QTDPEDIDOPZ,
								   NULL,
								   NULL,
								   NULL,
								   NULL,
								   NULL,
								   NULL,
								   NULL,
								   NULL,
								   NULL,
								   NULL,
								   NULL,
								   NULL,
								   NULL,
								   NULL,
								   NULL,
								   NULL,
								   @p_TOKEN_GERADO,
								   @p_TOKEN,
								   @p_ID_ITEM

		INSERT INTO tbLogPlanoZero (tipoPlanoZero, codTecnicoCliente, codPeca, dtHoraCriacao, idUsuarioCriacao, potenciaLPecas,
		qtdUltimoAno, qtdPZACalculada, qtdPedidoPZ, potencialTotal, qtdClientes, fatorPonderacao, nPedigoGerado, idPedido)
		VALUES ('T', @CD_TECNICO, @CD_PECA, GETDATE(), @ID_USUARIO, @POTENCIALPECA,
		@QTDULTIMOANO, @QTDPZACALCULADA, @QTDPEDIDOPZ, @MAQ_CART, @QT_CLIENTES, @FATORPONDERACAO, NULL, @p_ID_PEDIDO)

		  FETCH NEXT FROM @cursor2 INTO @cd_peca, @QTDPEDIDOPZ, @POTENCIALPECA, @QTDULTIMOANO, @QTDPZACALCULADA
		END
		CLOSE @cursor2
		DEALLOCATE @cursor2
		FETCH NEXT FROM @cursor INTO @CD_TECNICO
	  END
	  CLOSE @cursor
	  DEALLOCATE @cursor
 




 	--pega a lista de Clientes que estão ativos e tem sem plano zero 
	SET @cursor = CURSOR FOR
	SELECT distinct
	tt.CD_TECNICO, cl.CD_CLIENTE
	FROM TB_TECNICO tt
	INNER JOIN TB_TECNICO_CLIENTE tc
	ON tc.CD_TECNICO = tt.CD_TECNICO
	INNER JOIN TB_CLIENTE cl
	ON cl.CD_CLIENTE = tc.CD_CLIENTE
	WHERE tt.FL_ATIVO = 'S'
	AND tc.CD_ORDEM = 1
	AND (cl.FL_AtivaPlanoZero = 'S'
	OR cl.FL_AtivaPlanoZero IS NULL)
	order by tt.CD_TECNICO

	OPEN @cursor
		FETCH NEXT FROM @cursor INTO @CD_TECNICO, @CD_CLIENTE
			  WHILE @@FETCH_STATUS = 0
				BEGIN
					Set @cursor2 = CURSOR FOR
						SELECT PC.CD_PECA, pz.QT_PECA_MODELO, pc.QTD_PlanoZero
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
						AND MONTH(pz.DT_CRIACAO) = MONTH(GETDATE())
						AND YEAR(pz.DT_CRIACAO) = YEAR(GETDATE()) --REMOVER ANTES DE COMPILAR 
						AND TC.CD_TECNICO = @CD_TECNICO and cl.CD_CLIENTE = @cd_cliente 
						ORDER BY PC.CD_PECA
					OPEN @cursor2
					FETCH NEXT FROM @cursor2 INTO @CD_PECA, @POTENCIALPECA, @QTD_MINIMA
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
							@QTDULTIMOANO = Sum(tpc.QTD_RECEBIDA)
						FROM
							TB_PEDIDO tp
							INNER JOIN TB_PEDIDO_PECA tpc
							ON tp.ID_PEDIDO = tpc.ID_PEDIDO
							INNER JOIN TB_PECA pc
							ON tpc.CD_PECA = pc.CD_PECA
							INNER JOIN tbStatusPedido sp
							ON tp.ID_STATUS_PEDIDO = sp.ID_STATUS_PEDIDO
						WHERE
							YEAR(tp.DT_CRIACAO) = YEAR(DATEADD(YEAR, -1, GETDATE()))
							AND (tp.CD_TECNICO		= @CD_TECNICO)
							and sp.ID_STATUS_PEDIDO = 4
							and tpc.QTD_RECEBIDA is not null
							and tpc.CD_PECA = @CD_PECA
						

						--Pega o Potencial da Peça
						Select @POTENCIALPECA = tpp.POTENCIALPECA from tbPotencialPecas tpp where tpp.codPeca = @CD_PECA
						
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
						and tc.CD_CLIENTE = @CD_CLIENTE
						and tc.CD_TECNICO = @CD_TECNICO 

							--Calcula o Plano Zero Anual
							SET @QTDPZACALCULADA = (@QTDULTIMOANO/@POTENCIALPECA) * @MAQ_CART_TEC * (1 + @FATORPONDERACAO)

							--Valida a quantidade mínima do plano zero para a peça
							if @QTDPZACALCULADA/12 < @QTD_MINIMA
								set @QT_PZ_PERIODO = @QTD_MINIMA
							else
								Set @QT_PZ_PERIODO = ROUND(@QTDPZACALCULADA/12, 0)

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
							BEGIN TRANSACTION
								UPDATE tbPlanoZeroCliente
								SET potencialPecas = potencialPecas + @POTENCIALPECA, qtdUltimoAno = @QTDULTIMOANO
								WHERE codCliente = @CD_Cliente
								AND codPeca = @CD_PECA
								IF @@ROWCOUNT = 0
								BEGIN
								INSERT INTO tbPlanoZeroCliente (codCliente, codPeca, potencialPecas, qtdPZACalculada, qtdEstoque, qtdPedidoPZ, qtdUltimoAno)
								VALUES (@CD_CLIENTE, @CD_PECA, @POTENCIALPECA, @QTDPZACALCULADA, @QT_ESTOQUECLI, @QTDPEDIDOPZ, @QTDULTIMOANO)
								END
							COMMIT TRANSACTION	
						FETCH NEXT FROM @cursor2 INTO @CD_PECA, @POTENCIALPECA, @QTD_MINIMA
					END
  				FETCH NEXT FROM @cursor INTO @CD_TECNICO,  @CD_CLIENTE
				END
	CLOSE @cursor
	DEALLOCATE @cursor








	--pega a lista de Clientes que estão ativos e tem plano zero 
	SET @cursor = CURSOR FOR
		SELECT DISTINCT		
		cli.CD_Cliente
		FROM TB_CLIENTE cli 
		INNER JOIN tbPlanoZeroCliente pzt
		ON pzt.codCliente = cli.CD_CLIENTE
		ORDER BY cli.CD_CLIENTE

	 OPEN @cursor
	  FETCH NEXT FROM @cursor INTO @CD_CLIENTE
	  WHILE @@FETCH_STATUS = 0

  
	  BEGIN

	   EXEC prcPedidoInsert null,		        --@p_CD_TECNICO			 VARCHAR(6)		= NULL,
							 0,                 --@p_NR_DOCUMENTO		 NUMERIC(7,0)	= NULL,
							 @p_Data,           --@p_DT_CRIACAO			 DATETIME		= NULL,
							 NULL,              --@p_DT_ENVIO			 DATETIME		= NULL,
							 NULL,              --@p_DT_RECEBIMENTO		 DATETIME		= NULL,
							 NULL,              --@p_TX_OBS				 VARCHAR(255)	= NULL,
							 NULL,              --@p_PENDENTE			 VARCHAR(1)		= NULL,
							 NULL,              --@p_NR_DOC_ORI			 NUMERIC(18,0)	= NULL,
							 2,                 --@p_ID_STATUS_PEDIDO	 BIGINT			= NULL,
							 'C',               --@p_TP_TIPO_PEDIDO		 CHAR(1)		= NULL,
							 @CD_CLIENTE,              --@p_CD_CLIENTE			 NUMERIC(6,0)	= NULL,
							 @ID_USUARIO,       --@p_nidUsuarioAtualizacaoNUMERIC(18,0)	= NULL,
							 NULL,              --@p_TOKEN    			 BIGINT			= NULL,
							 'N',               --@p_TP_Especial			 varchar(1)		= NULL,
							 'Plano Zero Cliente',--@p_Responsavel			 varchar(70)    = null,
							 NULL,              --@p_Telefone			 varchar(12)	= null,
							 'W',              --@p_Origem				 varchar(1)		= null,
							 @p_TOKEN_GERADO,   --@p_TOKEN_GERADO    	 BIGINT		    OUTPUT,
							 @p_ID_PEDIDO       OUTPUT					


	   declare @count bigint;
	   SELECT
		  --@cd_peca = pzt.codPeca,
		  --@CD_CLIENTE = pzt.codCliente,
		  @count = (select count (*) FROM tbPlanoZeroCliente WHERE codCliente = @CD_CLIENTE)
		FROM tbPlanoZeroCliente pzt
		WHERE pzt.codCliente = @CD_CLIENTE
		AND pzt.qtdPedidoPZ > 0
		ORDER BY pzt.codPeca
	   declare FirstCursor cursor for SELECT
		  pzt.codPeca,
		  pzt.codCliente,
		  pzt.qtdPedidoPZ
		  --@count = (select count (*) FROM tbPlanoZeroCliente WHERE codCliente = @CD_CLIENTE)
		FROM tbPlanoZeroCliente pzt
		WHERE pzt.codCliente = @CD_CLIENTE
		AND pzt.qtdPedidoPZ > 0
		ORDER BY pzt.codPeca
	   open FirstCursor 
	   while @count > 0
		  begin
			 fetch FirstCursor into @cd_peca, @CD_CLIENTE, @QTDPEDIDOPZ
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
								   NULL,
								   1,
								   NULL,
								   NULL,
								   NULL,
								   @p_TOKEN_GERADO,
								   @p_ID_ITEM
			 set @count = @count - 1
		  end
	   close FirstCursor 
	   deallocate FirstCursor 

	
    FETCH NEXT FROM @cursor INTO @CD_TECNICO, @CD_CLIENTE
  END
  CLOSE @cursor
  DEALLOCATE @cursor

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

 