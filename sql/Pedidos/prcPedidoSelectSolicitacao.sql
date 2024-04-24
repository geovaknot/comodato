GO
/****** Object:  StoredProcedure [dbo].[prcPedidoSelectSolicitacao]    Script Date: 25/05/2022 15:51:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



ALTER PROCEDURE [dbo].[prcPedidoSelectSolicitacao]
	@p_CD_CLIENTE					NUMERIC(6,0)	= NULL,
	@p_CD_TECNICO					VARCHAR(06)		= NULL,
	@p_CD_PEDIDO					BIGINT			= NULL,
	@p_DT_CRIACAO_INICIO			DATETIME		= NULL,
	@p_DT_CRIACAO_FIM				DATETIME		= NULL,
	@p_ID_STATUS_PEDIDO				BIGINT			= NULL,
	@p_ID_STATUS_PEDIDO_ADICIONAL	BIGINT			= NULL

AS
BEGIN

	-- Declaração de Variáveis
	DECLARE @cdsErrorMessage	NVARCHAR(4000),
			@nidErrorSeverity	INT,
			@nidErrorState		INT,
			@STATUS_PEDIDO_PENDENTE BIGINT = 5,
			@STATUS_PEDIDO_RECEBIDO_PENDENCIA BIGINT = 6

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY

		DECLARE @STATUSCONFIRMAR NVARCHAR(MAX)	= NULL

		IF (@p_ID_STATUS_PEDIDO_ADICIONAL > 0) 
		BEGIN
			IF (@p_ID_STATUS_PEDIDO_ADICIONAL = @STATUS_PEDIDO_PENDENTE)
			BEGIN
				SET @STATUSCONFIRMAR = cast(@p_ID_STATUS_PEDIDO as VARCHAR(MAX)) + ',' + cast(@p_ID_STATUS_PEDIDO_ADICIONAL as VARCHAR(MAX)) + ',' + cast(@STATUS_PEDIDO_RECEBIDO_PENDENCIA as VARCHAR(MAX)) + ',';
			END
			ELSE
			BEGIN
				SET @STATUSCONFIRMAR = cast(@p_ID_STATUS_PEDIDO as VARCHAR(MAX)) + ',' + cast(@p_ID_STATUS_PEDIDO_ADICIONAL as VARCHAR(MAX)) + ',';
			END
		END
		ELSE
		BEGIN
			SET @STATUSCONFIRMAR = CAST(@p_ID_STATUS_PEDIDO AS VARCHAR(MAX)) + ',';
		END

		IF @p_CD_TECNICO IS NULL
		BEGIN
			SELECT 
				dbo.TB_PEDIDO.ID_PEDIDO,
				dbo.TB_PEDIDO.CD_PEDIDO,
				dbo.TB_PEDIDO.DT_CRIACAO,
				dbo.TB_PEDIDO.DT_Aprovacao,
				dbo.tbStatusPedido.ID_STATUS_PEDIDO,
				dbo.tbStatusPedido.DS_STATUS_PEDIDO,
				ISNULL(SUM(dbo.TB_PEDIDO_PECA.QTD_SOLICITADA), 0) AS QTD_SOLICITADA,
				dbo.TB_TECNICO.CD_TECNICO,
				dbo.TB_TECNICO.NM_TECNICO,
				dbo.TB_Empresa.CD_Empresa,
				dbo.TB_Empresa.Nm_Empresa,
				dbo.TB_PEDIDO.TP_TIPO_PEDIDO,
				dbo.TB_PEDIDO.FL_EMERGENCIA,
				dbo.TB_CLIENTE.CD_CLIENTE,
				dbo.TB_CLIENTE.NM_CLIENTE
			FROM dbo.TB_PEDIDO (nolock)
			LEFT OUTER JOIN dbo.TB_PEDIDO_PECA (nolock)
			ON dbo.TB_PEDIDO.ID_PEDIDO = dbo.TB_PEDIDO_PECA.ID_PEDIDO
			INNER JOIN dbo.tbStatusPedido (nolock)
			ON dbo.TB_PEDIDO.ID_STATUS_PEDIDO = dbo.tbStatusPedido.ID_STATUS_PEDIDO
			INNER JOIN dbo.TB_TECNICO (nolock)
			ON dbo.TB_PEDIDO.CD_TECNICO = dbo.TB_TECNICO.CD_TECNICO
			LEFT JOIN dbo.TB_CLIENTE (nolock)
			ON dbo.TB_PEDIDO.CD_CLIENTE = dbo.TB_CLIENTE.CD_CLIENTE
			LEFT OUTER JOIN dbo.TB_Empresa (nolock)
			ON dbo.TB_TECNICO.CD_EMPRESA = dbo.TB_Empresa.CD_Empresa
			WHERE
					( dbo.TB_PEDIDO.CD_CLIENTE			= @p_CD_CLIENTE					OR		@p_CD_CLIENTE					IS NULL )
				AND	( dbo.TB_PEDIDO.CD_TECNICO			= @p_CD_TECNICO					OR		@p_CD_TECNICO					IS NULL )
				AND ( dbo.TB_PEDIDO.CD_PEDIDO			= @p_CD_PEDIDO					OR		@p_CD_PEDIDO					IS NULL )
				AND ( dbo.TB_PEDIDO.DT_CRIACAO			>= @p_DT_CRIACAO_INICIO			OR		@p_DT_CRIACAO_INICIO			IS NULL )
				AND	( dbo.TB_PEDIDO.DT_CRIACAO			<= @p_DT_CRIACAO_FIM			OR		@p_DT_CRIACAO_FIM				IS NULL )
				AND ( dbo.TB_PEDIDO.ID_STATUS_PEDIDO	IN (SELECT cdsString FROM dbo.fncGetValuesByString(@STATUSCONFIRMAR, ',')) OR @p_ID_STATUS_PEDIDO IS NULL)
			GROUP BY
				dbo.TB_PEDIDO.ID_PEDIDO,
				dbo.TB_PEDIDO.CD_PEDIDO,
				dbo.TB_PEDIDO.DT_CRIACAO,
				dbo.TB_PEDIDO.DT_Aprovacao,
				dbo.tbStatusPedido.ID_STATUS_PEDIDO,
				dbo.tbStatusPedido.DS_STATUS_PEDIDO,
				dbo.TB_TECNICO.CD_TECNICO,
				dbo.TB_TECNICO.NM_TECNICO,
				dbo.TB_Empresa.CD_Empresa,
				dbo.TB_Empresa.Nm_Empresa,
				dbo.TB_PEDIDO.TP_TIPO_PEDIDO,
				dbo.TB_PEDIDO.FL_EMERGENCIA,
				dbo.TB_CLIENTE.CD_CLIENTE,
				dbo.TB_CLIENTE.NM_CLIENTE
			ORDER BY 
				dbo.TB_PEDIDO.DT_CRIACAO DESC,
				dbo.TB_PEDIDO.CD_PEDIDO DESC
		
		END
		ELSE
		BEGIN
			-- Consulta de pedidos com ID_STATUS_PEDIDO = 1 (Novo/Rascunho) apenas o dono do pedido (CD_TECNICO) pode visualizá-lo
			SELECT 
				dbo.TB_PEDIDO.ID_PEDIDO,
				dbo.TB_PEDIDO.CD_PEDIDO,
				dbo.TB_PEDIDO.DT_CRIACAO,
				dbo.TB_PEDIDO.DT_Aprovacao,
				dbo.tbStatusPedido.ID_STATUS_PEDIDO,
				dbo.tbStatusPedido.DS_STATUS_PEDIDO,
				ISNULL(SUM(dbo.TB_PEDIDO_PECA.QTD_SOLICITADA), 0) AS QTD_SOLICITADA,
				dbo.TB_TECNICO.CD_TECNICO,
				dbo.TB_TECNICO.NM_TECNICO,
				dbo.TB_Empresa.CD_Empresa,
				dbo.TB_Empresa.Nm_Empresa,
				dbo.TB_PEDIDO.TP_TIPO_PEDIDO,
				dbo.TB_PEDIDO.FL_EMERGENCIA,
				dbo.TB_CLIENTE.CD_CLIENTE,
				dbo.TB_CLIENTE.NM_CLIENTE
			FROM dbo.TB_PEDIDO (NOLOCK)
			LEFT OUTER JOIN dbo.TB_PEDIDO_PECA (NOLOCK)
			ON dbo.TB_PEDIDO.ID_PEDIDO = dbo.TB_PEDIDO_PECA.ID_PEDIDO
			INNER JOIN dbo.tbStatusPedido (NOLOCK)
			ON dbo.TB_PEDIDO.ID_STATUS_PEDIDO = dbo.tbStatusPedido.ID_STATUS_PEDIDO
			INNER JOIN dbo.TB_TECNICO (NOLOCK)
			ON dbo.TB_PEDIDO.CD_TECNICO = dbo.TB_TECNICO.CD_TECNICO
			LEFT JOIN dbo.TB_CLIENTE (nolock)
			ON dbo.TB_PEDIDO.CD_CLIENTE = dbo.TB_CLIENTE.CD_CLIENTE
			LEFT OUTER JOIN dbo.TB_Empresa (NOLOCK)
			ON dbo.TB_TECNICO.CD_EMPRESA = dbo.TB_Empresa.CD_Empresa
			WHERE
					( dbo.TB_PEDIDO.CD_CLIENTE			= @p_CD_CLIENTE					OR		@p_CD_CLIENTE			IS NULL )
				AND ( dbo.TB_PEDIDO.CD_PEDIDO			= @p_CD_PEDIDO					OR		@p_CD_PEDIDO			IS NULL )
				AND ( dbo.TB_PEDIDO.DT_CRIACAO			>= @p_DT_CRIACAO_INICIO			OR		@p_DT_CRIACAO_INICIO	IS NULL )
				AND	( dbo.TB_PEDIDO.DT_CRIACAO			<= @p_DT_CRIACAO_FIM			OR		@p_DT_CRIACAO_FIM		IS NULL )
				AND ( dbo.TB_PEDIDO.ID_STATUS_PEDIDO	IN (SELECT cdsString FROM dbo.fncGetValuesByString(@STATUSCONFIRMAR, ',')) OR @p_ID_STATUS_PEDIDO IS NULL)
				AND ( dbo.TB_PEDIDO.ID_STATUS_PEDIDO	= 1 )
				AND ( dbo.TB_PEDIDO.CD_TECNICO			= @p_CD_TECNICO )
			GROUP BY
				dbo.TB_PEDIDO.ID_PEDIDO,
				dbo.TB_PEDIDO.CD_PEDIDO,
				dbo.TB_PEDIDO.DT_CRIACAO,
				dbo.TB_PEDIDO.DT_Aprovacao,
				dbo.tbStatusPedido.ID_STATUS_PEDIDO,
				dbo.tbStatusPedido.DS_STATUS_PEDIDO,
				dbo.TB_TECNICO.CD_TECNICO,
				dbo.TB_TECNICO.NM_TECNICO,
				dbo.TB_Empresa.CD_Empresa,
				dbo.TB_Empresa.Nm_Empresa,
				dbo.TB_PEDIDO.TP_TIPO_PEDIDO,
				dbo.TB_PEDIDO.FL_EMERGENCIA,
				dbo.TB_CLIENTE.CD_CLIENTE,
				dbo.TB_CLIENTE.NM_CLIENTE
			-- Já os demais ST_STATUS_PEDIDO, todos podem visualizá-los
			UNION 
			SELECT 
				dbo.TB_PEDIDO.ID_PEDIDO,
				dbo.TB_PEDIDO.CD_PEDIDO,
				dbo.TB_PEDIDO.DT_CRIACAO,
				dbo.TB_PEDIDO.DT_Aprovacao,
				dbo.tbStatusPedido.ID_STATUS_PEDIDO,
				dbo.tbStatusPedido.DS_STATUS_PEDIDO,
				ISNULL(SUM(dbo.TB_PEDIDO_PECA.QTD_SOLICITADA), 0) AS QTD_SOLICITADA,
				dbo.TB_TECNICO.CD_TECNICO,
				dbo.TB_TECNICO.NM_TECNICO,
				dbo.TB_Empresa.CD_Empresa,
				dbo.TB_Empresa.Nm_Empresa,
				dbo.TB_PEDIDO.TP_TIPO_PEDIDO,
				dbo.TB_PEDIDO.FL_EMERGENCIA,
				dbo.TB_CLIENTE.CD_CLIENTE,
				dbo.TB_CLIENTE.NM_CLIENTE
			FROM dbo.TB_PEDIDO
			LEFT OUTER JOIN dbo.TB_PEDIDO_PECA
			ON dbo.TB_PEDIDO.ID_PEDIDO = dbo.TB_PEDIDO_PECA.ID_PEDIDO
			INNER JOIN dbo.tbStatusPedido
			ON dbo.TB_PEDIDO.ID_STATUS_PEDIDO = dbo.tbStatusPedido.ID_STATUS_PEDIDO
			INNER JOIN dbo.TB_TECNICO
			ON dbo.TB_PEDIDO.CD_TECNICO = dbo.TB_TECNICO.CD_TECNICO
			LEFT JOIN dbo.TB_CLIENTE (nolock)
			ON dbo.TB_PEDIDO.CD_CLIENTE = dbo.TB_CLIENTE.CD_CLIENTE
			LEFT OUTER JOIN dbo.TB_Empresa
			ON dbo.TB_TECNICO.CD_EMPRESA = dbo.TB_Empresa.CD_Empresa
			WHERE
					( dbo.TB_PEDIDO.CD_CLIENTE			= @p_CD_CLIENTE					OR		@p_CD_CLIENTE			IS NULL )
				AND	( dbo.TB_PEDIDO.CD_TECNICO			= @p_CD_TECNICO					OR		@p_CD_TECNICO			IS NULL )
				AND ( dbo.TB_PEDIDO.CD_PEDIDO			= @p_CD_PEDIDO					OR		@p_CD_PEDIDO			IS NULL )
				AND ( dbo.TB_PEDIDO.DT_CRIACAO			>= @p_DT_CRIACAO_INICIO			OR		@p_DT_CRIACAO_INICIO	IS NULL )
				AND	( dbo.TB_PEDIDO.DT_CRIACAO			<= @p_DT_CRIACAO_FIM			OR		@p_DT_CRIACAO_FIM		IS NULL )
				AND ( dbo.TB_PEDIDO.ID_STATUS_PEDIDO	IN (SELECT cdsString FROM dbo.fncGetValuesByString(@STATUSCONFIRMAR, ',')) OR @p_ID_STATUS_PEDIDO IS NULL)
				AND ( dbo.TB_PEDIDO.ID_STATUS_PEDIDO	<> 1 )
			GROUP BY
				dbo.TB_PEDIDO.ID_PEDIDO,
				dbo.TB_PEDIDO.CD_PEDIDO,
				dbo.TB_PEDIDO.DT_CRIACAO,
				dbo.TB_PEDIDO.DT_Aprovacao,
				dbo.tbStatusPedido.ID_STATUS_PEDIDO,
				dbo.tbStatusPedido.DS_STATUS_PEDIDO,
				dbo.TB_TECNICO.CD_TECNICO,
				dbo.TB_TECNICO.NM_TECNICO,
				dbo.TB_Empresa.CD_Empresa,
				dbo.TB_Empresa.Nm_Empresa,
				dbo.TB_PEDIDO.TP_TIPO_PEDIDO,
				dbo.TB_PEDIDO.FL_EMERGENCIA,
				dbo.TB_CLIENTE.CD_CLIENTE,
				dbo.TB_CLIENTE.NM_CLIENTE
			ORDER BY 
				dbo.TB_PEDIDO.DT_CRIACAO DESC,
				dbo.TB_PEDIDO.CD_PEDIDO DESC
		
		END				
	END TRY

	BEGIN CATCH

		SELECT	@cdsErrorMessage	= ERROR_MESSAGE(),
				@nidErrorSeverity	= ERROR_SEVERITY(),
				@nidErrorState		= ERROR_STATE();

		-- Use RAISERROR inside the CATCH block to return error
		-- information about the original error that caused
		-- execution to jump to the CATCH block.
		RAISERROR (@cdsErrorMessage, -- Message text.
				   @nidErrorSeverity, -- Severity.
				   @nidErrorState -- State.
				   )

	END CATCH

END


