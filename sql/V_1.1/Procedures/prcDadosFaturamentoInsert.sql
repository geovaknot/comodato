GO
/****** Object:  StoredProcedure [dbo].[prcDadosFaturamentoInsert]    Script Date: 21/02/2022 10:38:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Caio Carneiro
-- Create date: 19/03/2018
-- Description:	Consulta dos dados na tabela
--              tbPecaOS
-- =============================================
ALTER PROCEDURE [dbo].[prcDadosFaturamentoInsert]
	@p_CD_CLIENTE					VARCHAR(8)		= NULL,
	@p_NRAtivo						VARCHAR(15)		= NULL,
	@p_CD_Material					VARCHAR(15)		= NULL,
	@p_DepartamentoVenda			VARCHAR(2)		= NULL,
	@p_AluguelApos3Anos				NUMERIC(15,2)	= NULL,
	@p_DT_UltimoFaturamento			DATETIME		= NULL,
	@p_nidUsuarioSolicitacao		NUMERIC(8,0)	= NULL,
	@p_DT_Solicitacao				DATETIME		= NULL,
	@p_HR_solicitacao				VARCHAR(5)		= NULL,
	@p_AtivoFixo					VARCHAR(6)		= NULL,
	@p_ID_ATIVO_CLIENTE				BIGINT			= NULL
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
		
		INSERT INTO dbo.TB_DadosFaturamento
		(CD_Cliente, NRAtivo, CD_Material, DepartamentoVenda, AluguelApos3anos,DT_UltimoFaturamento,nidUsuarioSolicitacao, DT_Solicitacao, HR_solicitacao, ID_ATIVO_CLIENTE, EnviadoBpcs, Ativo, SituacaoBpcs, AtivoFixo)
		VALUES
		(@p_CD_Cliente, @p_NRAtivo, @p_CD_Material, @p_DepartamentoVenda, @p_AluguelApos3anos,@p_DT_UltimoFaturamento,@p_nidUsuarioSolicitacao, @p_DT_Solicitacao, @p_HR_solicitacao, @p_ID_ATIVO_CLIENTE, 0, 1, 'A', @p_AtivoFixo)
		
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


