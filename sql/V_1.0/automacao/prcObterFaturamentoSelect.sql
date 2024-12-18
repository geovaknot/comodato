GO
/****** Object:  StoredProcedure [dbo].[prcObterFaturamentoSelect]    Script Date: 15/12/2021 11:30:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Flavio Ribeiro
-- Create date: 
-- Description:	Seleção de dados na tabela tbEstoquePeca
-- =============================================
CREATE PROCEDURE [dbo].[prcObterFaturamentoSelect]
	@p_ID		BIGINT			= NULL
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
		
		SELECT	*	
		FROM	TB_DadosFaturamento
		
		WHERE 
			ID = @p_ID

		--FROM	dbo.TB_PECA
		--LEFT JOIN tbEstoquePeca p	ON dbo.TB_PECA.CD_PECA = p.CD_PECA
	
		--LEFT JOIN tbEstoque e	on p.ID_ESTOQUE = e.ID_ESTOQUE
			
		--	and (@p_TP_ESTOQUE_TEC_3M IS NULL OR RTRIM(LTRIM(e.TP_ESTOQUE_TEC_3M)) IN (SELECT cdsString FROM fncGetValuesByString(@p_TP_ESTOQUE_TEC_3M,',')))

		--WHERE 
		--	(p.ID_ESTOQUE_PECA	= @p_ID_ESTOQUE_PECA	or @p_ID_ESTOQUE_PECA	is null)	
		--AND (p.CD_PECA			= @p_CD_PECA			or @p_CD_PECA			is null)
		--and (p.QT_PECA_ATUAL	= @p_QT_PECA_ATUAL		or @p_QT_PECA_ATUAL		is null)
		--and (p.QT_PECA_MIN		= @p_QT_PECA_MIN		or @p_QT_PECA_MIN		is null)
		--and (p.DT_ULT_MOVIM		= @p_DT_ULT_MOVIM		or @p_DT_ULT_MOVIM		is null)
		--and (p.ID_ESTOQUE		= @p_ID_ESTOQUE 		or @p_ID_ESTOQUE		is null)
		--and (e.FL_ATIVO			= @p_FL_ATIVO			OR @p_FL_ATIVO			IS NULL)

		--Teste André - Remover peças c qtd zero:
		--AND	(p.QT_PECA_ATUAL > 0)
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





