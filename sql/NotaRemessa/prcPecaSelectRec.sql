GO
/****** Object:  StoredProcedure [dbo].[prcPecaSelectRec]    Script Date: 14/09/2022 11:59:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Caio Carneiro
-- Create date: 19/03/2018
-- Description:	Seleção de dados na tabela 
--              TB_PECA
-- =============================================
CREATE PROCEDURE [dbo].[prcPecaSelectRec]
	@p_CD_PECA							VARCHAR(15) = null
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
			UPPER(tb_PECA.CD_PECA) AS CD_PECA,
			DS_PECA,
			TX_UNIDADE,
			QTD_ESTOQUE,
			QTD_MINIMA,
			VL_PECA,
			TP_PECA,
			FL_ATIVO_PECA,
			tbPecaRecuperada.CD_PECA_RECUPERADA
		FROM	TB_PECA with(nolock)
		left join tbPecaRecuperada on 
		tbPecaRecuperada.CD_PECA = TB_PECA.CD_PECA
		WHERE (	tb_PECA.CD_PECA			= @p_CD_PECA		OR @p_CD_PECA		IS NULL )
		
			 
		
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


