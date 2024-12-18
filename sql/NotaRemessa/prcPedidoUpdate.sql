GO
/****** Object:  StoredProcedure [dbo].[prcPedidoUpdate]    Script Date: 04/10/2022 15:46:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Caio Carneiro
-- Create date: 19/03/2018
-- Description:	Update dos dados na tabela
--               TB_PEDIDO
-- =============================================
ALTER PROCEDURE [dbo].[prcPedidoUpdate]
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
		@p_FL_EMERGENCIA					CHAR(1)			= NULL,
		@p_Responsavel						VARCHAR(70)		= NULL,
		@p_Telefone							Varchar(12)		= NULL,
		@p_EnviaBPCS						Varchar(1)		= NULL,
		@p_nidUsuarioAtualizacao			NUMERIC(18,0)	= NULL,
		@p_nidUsuario						NUMERIC(18,0)	= NULL
AS
BEGIN

	-- Declaração de Variáveis
	DECLARE @cdsErrorMessage	NVARCHAR(4000),
			@nidErrorSeverity	INT,
			@nidErrorState		INT,
			@nidLog				NUMERIC(18,0)

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY

		--BEGIN TRANSACTION
		
		EXECUTE dbo.prcLogGravar 
				@p_nidLog					= @nidLog,
				@p_nidUsuarioAtualizacao	= @p_nidUsuarioAtualizacao,
				@p_ccdAcao					= 'U',
				@p_cnmTabela				= 'TB_PEDIDO',
				@p_nidPK					= @p_ID_PEDIDO,
				@p_nidLogReturn				= @nidLog OUTPUT
				
		UPDATE	TB_PEDIDO SET						
			CD_TECNICO			= ISNULL(@p_CD_TECNICO,			CD_TECNICO),
			NR_DOCUMENTO		= ISNULL(@p_NR_DOCUMENTO,		NR_DOCUMENTO),	
			DT_CRIACAO			= ISNULL(@p_DT_CRIACAO,			DT_CRIACAO),
			DT_ENVIO			= ISNULL(@p_DT_ENVIO,			DT_ENVIO),
			DT_RECEBIMENTO		= ISNULL(@p_DT_RECEBIMENTO,		DT_RECEBIMENTO),	
			TX_OBS				= ISNULL(@p_TX_OBS,				TX_OBS),
			PENDENTE			= ISNULL(@p_PENDENTE,			PENDENTE),
			NR_DOC_ORI			= ISNULL(@p_NR_DOC_ORI,			NR_DOC_ORI),
			ID_STATUS_PEDIDO	= ISNULL(@p_ID_STATUS_PEDIDO,	ID_STATUS_PEDIDO),
			CD_CLIENTE			= ISNULL(@p_CD_CLIENTE,			CD_CLIENTE),
			CD_PEDIDO			= ISNULL(@p_CD_PEDIDO,			CD_PEDIDO),
			FL_EMERGENCIA		= ISNULL(@p_FL_EMERGENCIA,		FL_EMERGENCIA),
			Responsavel			= ISNULL(@p_Responsavel,		Responsavel),
			Telefone			= ISNULL(@p_Telefone,			Telefone),
			EnviaBPCS			= ISNULL(@p_EnviaBPCS,			EnviaBPCS),
			nidUsuario			= ISNULL(@p_nidUsuario,			nidUsuario)
		WHERE ID_PEDIDO			= @p_ID_PEDIDO
		
		IF @p_CD_CLIENTE = -1
		BEGIN
			UPDATE dbo.TB_PEDIDO SET CD_CLIENTE = NULL
			WHERE ID_PEDIDO		= @p_ID_PEDIDO          
		END      

		EXECUTE dbo.prcLogGravar 
				@p_nidLog					= @nidLog,
				@p_nidUsuarioAtualizacao	= @p_nidUsuarioAtualizacao,
				@p_ccdAcao					= 'U',
				@p_cnmTabela				= 'TB_PEDIDO',
				@p_nidPK					= @p_ID_PEDIDO,
				@p_nidLogReturn				= @nidLog OUTPUT
	
		--COMMIT TRANSACTION
	
	END TRY

	BEGIN CATCH

		SELECT	@cdsErrorMessage	= ERROR_MESSAGE(),
				@nidErrorSeverity	= ERROR_SEVERITY(),
				@nidErrorState		= ERROR_STATE();

		--ROLLBACK TRANSACTION

		-- Use RAISERROR inside the CATCH block to return error
		-- information about the original error that caused
		-- execution to jump to the CATCH block.
		RAISERROR (@cdsErrorMessage, -- Message text.
				   @nidErrorSeverity, -- Severity.
				   @nidErrorState -- State.
				   )

	END CATCH

END

