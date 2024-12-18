GO
/****** Object:  StoredProcedure [dbo].[prcPedidoUpdateBPCS]    Script Date: 04/10/2022 15:51:03 ******/
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
ALTER PROCEDURE [dbo].[prcPedidoUpdateBPCS]
		@p_ID_PEDIDO						NUMERIC(9,0)	= NULL,
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
			Responsavel			= ISNULL(@p_Responsavel,		Responsavel),
			Telefone			= ISNULL(@p_Telefone,			Telefone),
			EnviaBPCS			= ISNULL(@p_EnviaBPCS,			EnviaBPCS),
			DT_Aprovacao		= GETDATE(),
			nidUsuario			= ISNULL(@p_nidUsuario,			nidUsuario)
		WHERE ID_PEDIDO			= @p_ID_PEDIDO
		
		
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

