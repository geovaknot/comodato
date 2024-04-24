GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Caio Carneiro
-- Create date: 19/03/2018
-- Description:	Seleção de dados na tabela 
--              TB_PEDIDO
-- =============================================
ALTER PROCEDURE [dbo].[prcPedidoSelect]
		@p_ID_PEDIDO						NUMERIC(9,0)	= NULL,
		@p_CD_TECNICO						VARCHAR(6)		= NULL,
		@p_NR_DOCUMENTO						NUMERIC(7,0)	= NULL,
		@p_DT_CRIACAO						DATETIME		= NULL,
		@p_DT_ENVIO							DATETIME		= NULL,
		@p_DT_RECEBIMENTO					DATETIME		= NULL,
		@p_TX_OBS							VARCHAR(255)	= NULL,
		@p_PENDENTE							VARCHAR(1)		= NULL,
		@p_NR_DOC_ORI						NUMERIC(18,0)	= NULL,
		@p_ID_STATUS_PEDIDO					BIGINT			= NULL,
		@p_CD_CLIENTE						NUMERIC(6,0)	= NULL,
		@p_CD_PEDIDO						BIGINT			= NULL,
		@p_nidUsuarioAtualizacao			NUMERIC(18,0)	= NULL

AS
BEGIN

	-- Declaração de Variáveis
	DECLARE @cdsErrorMessage	NVARCHAR(4000),
			@nidErrorSeverity	INT,
			@nidErrorState		INT

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY
		
		SELECT	TB_PEDIDO.*,
			dbo.tbStatusPedido.DS_STATUS_PEDIDO,
			dbo.TB_TECNICO.NM_TECNICO,
			dbo.TB_TECNICO.ID_USUARIO,
			dbo.TB_Empresa.CD_Empresa,
			dbo.TB_Empresa.Nm_Empresa

		FROM	TB_PEDIDO(nolock)
		INNER JOIN dbo.tbStatusPedido(nolock)
		ON dbo.TB_PEDIDO.ID_STATUS_PEDIDO = dbo.tbStatusPedido.ID_STATUS_PEDIDO
		INNER JOIN dbo.TB_TECNICO(nolock)
		ON dbo.TB_PEDIDO.CD_TECNICO = dbo.TB_TECNICO.CD_TECNICO
		LEFT OUTER JOIN dbo.TB_Empresa(nolock)
		ON dbo.TB_TECNICO.CD_EMPRESA = dbo.TB_Empresa.CD_Empresa
		WHERE (	TB_PEDIDO.ID_PEDIDO			= @p_ID_PEDIDO			OR @p_ID_PEDIDO			IS NULL )
		AND	  (	TB_PEDIDO.CD_TECNICO		= @p_CD_TECNICO			OR @p_CD_TECNICO		IS NULL )
		AND	  (	TB_PEDIDO.NR_DOCUMENTO		= @p_NR_DOCUMENTO		OR @p_NR_DOCUMENTO		IS NULL )
		AND	  (	TB_PEDIDO.DT_CRIACAO		>= @p_DT_CRIACAO		OR @p_DT_CRIACAO		IS NULL )
		AND	  (	TB_PEDIDO.DT_ENVIO			= @p_DT_ENVIO			OR @p_DT_ENVIO			IS NULL )
		AND	  (	TB_PEDIDO.DT_RECEBIMENTO	= @p_DT_RECEBIMENTO		OR @p_DT_RECEBIMENTO	IS NULL )
		AND	  (	TB_PEDIDO.TX_OBS			LIKE @p_TX_OBS			OR @p_TX_OBS			IS NULL )
		AND   ( TB_PEDIDO.PENDENTE			= @p_PENDENTE			OR @p_PENDENTE			IS NULL )
		AND	  ( TB_PEDIDO.NR_DOC_ORI		= @p_NR_DOC_ORI			OR @p_NR_DOC_ORI		IS NULL )
		AND	  ( TB_PEDIDO.ID_STATUS_PEDIDO	= @p_ID_STATUS_PEDIDO	OR @p_ID_STATUS_PEDIDO	IS NULL )
		AND	  ( TB_PEDIDO.CD_CLIENTE		= @p_CD_CLIENTE			OR @p_CD_CLIENTE		IS NULL )
		AND   ( TB_PEDIDO.CD_PEDIDO			= @p_CD_PEDIDO			OR @p_CD_PEDIDO			IS NULL )
		AND	    NOT TB_PEDIDO.CD_PEDIDO		IS NULL 
		ORDER BY
				TB_PEDIDO.CD_PEDIDO DESC			 
		
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