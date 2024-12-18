GO
/****** Object:  StoredProcedure [dbo].[prcEstoqueSelect]    Script Date: 22/07/2021 09:10:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Flavio Ribeiro
-- Create date: 17/05/2018
-- Description:	Seleção de dados na tabela tbEstoque
-- =============================================
ALTER PROCEDURE [dbo].[prcEstoqueSelect]
	 @p_ID_ESTOQUE			bigint = NULL
	,@p_CD_ESTOQUE			char(10) = NULL
	,@p_DS_ESTOQUE			char(150) = NULL
	,@p_ID_USU_RESPONSAVEL	bigint = null
	,@p_DT_CRIACAO			datetime = NULL
	,@p_CD_TECNICO			varchar(6) = NULL
	,@p_TP_ESTOQUE_TEC_3M	nchar(3) = NULL
	,@p_FL_ATIVO			char(1) = null
	,@p_CD_CLIENTE			NUMERIC(6)=NULL
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
		
		SELECT	 ID_ESTOQUE
				,CD_ESTOQUE
				,DS_ESTOQUE
				,ID_USU_RESPONSAVEL
				--,cnmNome NM_TEC_RESPONSAVEL
				,DT_CRIACAO
				,CD_TECNICO
				,TP_ESTOQUE_TEC_3M
				,ISNULL(FL_ATIVO,'') FL_ATIVO
				,CD_CLIENTE
		
		FROM	tbEstoque 
			WHERE ID_USU_RESPONSAVEL IN (SELECT nidUSuario FROM fncRetornaUsuariosAcesso(@p_ID_USU_RESPONSAVEL))
			AND (ID_ESTOQUE = @p_ID_ESTOQUE OR @p_ID_ESTOQUE IS NULL)
			AND (FL_ATIVO = @p_FL_ATIVO OR @p_FL_ATIVO IS NULL)
		
		ORDER BY
			DS_ESTOQUE      

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





