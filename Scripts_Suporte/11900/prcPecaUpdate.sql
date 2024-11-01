GO
/****** Object:  StoredProcedure [dbo].[prcPecaUpdate]    Script Date: 25/04/2023 19:56:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Caio Carneiro
-- Create date: 19/03/2018
-- Description:	Update dos dados na tabela
--               TB_PECA
-- =============================================
ALTER PROCEDURE [dbo].[prcPecaUpdate]
		@p_CD_PECA							VARCHAR(15) = NULL,
		@p_DS_PECA							VARCHAR(50) = NULL,
		@p_TX_UNIDADE						VARCHAR(2) = NULL,
		@p_QTD_ESTOQUE						NUMERIC(15,3) = NULL,
		@p_QTD_MINIMA						NUMERIC(15,3) = NULL,
		@p_VL_PECA							NUMERIC(14,2) = NULL,
		@p_TP_PECA							VARCHAR(1) = NULL,
		@p_FL_ATIVO_PECA					VARCHAR(1) = NULL,
		@p_nidUsuarioAtualizacao			NUMERIC(18,0)	= NULL,
		@p_QTD_PlanoZero					Numeric(6,0) = Null
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

		BEGIN TRANSACTION
		
		EXECUTE dbo.prcLogGravar 
				@p_nidLog					= @nidLog,
				@p_nidUsuarioAtualizacao	= @p_nidUsuarioAtualizacao,
				@p_ccdAcao					= 'U',
				@p_cnmTabela				= 'TB_PECA',
				@p_nidPK					= @p_CD_PECA,
				@p_nidLogReturn				= @nidLog OUTPUT
				
		UPDATE	TB_PECA
		SET							
			DS_PECA			= @p_DS_PECA,	
			TX_UNIDADE		= @p_TX_UNIDADE,	
			QTD_ESTOQUE		= @p_QTD_ESTOQUE,	
			QTD_MINIMA		= @p_QTD_MINIMA,	
			VL_PECA			= @p_VL_PECA,	
			TP_PECA			= @p_TP_PECA,	
			FL_ATIVO_PECA	= @p_FL_ATIVO_PECA,
			QTD_PlanoZero	= @p_QTD_PlanoZero
		WHERE CD_PECA = @p_CD_PECA
		         	
		EXECUTE dbo.prcLogGravar 
				@p_nidLog					= @nidLog,
				@p_nidUsuarioAtualizacao	= @p_nidUsuarioAtualizacao,
				@p_ccdAcao					= 'U',
				@p_cnmTabela				= 'TB_PECA',
				@p_nidPK					= @p_CD_PECA,
				@p_nidLogReturn				= @nidLog OUTPUT
	
		COMMIT TRANSACTION
	
	END TRY

	BEGIN CATCH

		SELECT	@cdsErrorMessage	= ERROR_MESSAGE(),
				@nidErrorSeverity	= ERROR_SEVERITY(),
				@nidErrorState		= ERROR_STATE();

		ROLLBACK TRANSACTION

		-- Use RAISERROR inside the CATCH block to return error
		-- information about the original error that caused
		-- execution to jump to the CATCH block.
		RAISERROR (@cdsErrorMessage, -- Message text.
				   @nidErrorSeverity, -- Severity.
				   @nidErrorState -- State.
				   )

	END CATCH

END

