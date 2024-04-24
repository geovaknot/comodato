GO
/****** Object:  StoredProcedure [dbo].[prcEstoqueUsuarioSelectADM]    Script Date: 02/08/2021 08:42:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[prcEstoqueUsuarioSelectADM]
	@p_nidEstoque bigint = NULL,
	@p_nidUsuario NUMERIC(18,0),
	@p_FL_ATIVO CHAR(1) = NULL
AS
BEGIN

	-- Declaração de Variáveis
	DECLARE @cdsErrorMessage	NVARCHAR(4000),
			@nidErrorSeverity	INT,
			@nidErrorState		INT

	SET NOCOUNT ON;

	BEGIN TRY
		SELECT   ID_ESTOQUE
				,CD_ESTOQUE
				,DS_ESTOQUE
				,ID_USU_RESPONSAVEL
				,DT_CRIACAO
				,CD_TECNICO
				,TP_ESTOQUE_TEC_3M
				,FL_ATIVO
		FROM dbo.tbEstoque 
		WHERE ID_USU_RESPONSAVEL IN (SELECT nidUSuario FROM fncRetornaUsuariosAcesso(@p_nidUsuario))
		AND (ID_ESTOQUE = @p_nidEstoque OR @p_nidEstoque IS NULL)
		AND (FL_ATIVO = @p_FL_ATIVO OR @p_FL_ATIVO IS NULL)


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
