GO
/****** Object:  StoredProcedure [dbo].[prcRptEstoqueIntermediario]    Script Date: 18/10/2021 14:46:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ================================================
-- Author:		André Farinelli
-- Create date: 
-- Description:	
-- ================================================
ALTER PROCEDURE [dbo].[prcRptEstoqueIntermediario]
     @pCD_PECA				VARCHAR(15) = NULL
    ,@pFL_ATIVO_PECA		VARCHAR(1)	= NULL
	,@pCD_ESTOQUE			bigint	= Null
    ,@pFL_ATIVO_ESTOQUE		VARCHAR(1)	= NULL
    --,@pCD_TECNICO			VARCHAR(6)	= NULL
    --,@pFL_ATIVO_TECNICO	VARCHAR(1)	= NULL
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

		SELECT	
			--TC.CD_TECNICO,
			--TC.NM_TECNICO,
			E.CD_ESTOQUE,
			E.DS_ESTOQUE,
			PC.CD_PECA,
			PC.DS_PECA,
			QT_PECA_ATUAL QTD_ALOCADA, 
			QT_PECA_ATUAL * VL_PECA VL_TOTAL
		FROM 	
			tbEstoquePeca EP (NOLOCK)
		INNER JOIN tbEstoque E (NOLOCK)
			ON E.ID_ESTOQUE = EP.ID_ESTOQUE
			--AND E.TP_ESTOQUE_TEC_3M = 'TEC'
		INNER JOIN TB_PECA PC (NOLOCK)
			ON EP.CD_PECA = PC.CD_PECA  
			AND (@pCD_PECA			IS NULL OR PC.CD_PECA		= @pCD_PECA)
			AND (@pFL_ATIVO_PECA	IS NULL OR PC.FL_ATIVO_PECA = @pFL_ATIVO_PECA)
		--INNER JOIN TB_TECNICO TC (NOLOCK)
		--	ON E.CD_TECNICO = TC.CD_TECNICO 
		--	AND (@pCD_TECNICO		IS NULL OR TC.CD_TECNICO	= @pCD_TECNICO)
		--	AND (@pFL_ATIVO_TECNICO IS NULL OR TC.FL_ATIVO		= @pFL_ATIVO_TECNICO)
		WHERE 
			QT_PECA_ATUAL <> 0 
		AND (@pCD_ESTOQUE			IS NULL OR E.ID_ESTOQUE		= @pCD_ESTOQUE)
		AND (@pFL_ATIVO_ESTOQUE		IS NULL OR E.FL_ATIVO		= @pFL_ATIVO_ESTOQUE)

		--Order by TC.CD_TECNICO,	PC.DS_PECA	
		ORDER BY 
			CD_ESTOQUE, 
			DS_PECA	

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


