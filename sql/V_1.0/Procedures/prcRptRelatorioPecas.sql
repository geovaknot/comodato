GO
/****** Object:  StoredProcedure [dbo].[prcRptRelatorioPecas]    Script Date: 18/10/2021 09:45:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ================================================
-- Author:		Paulo Rabelo
-- Alter date: 06/09/2019
-- Description:	Relatório Kat por Técnico
-- ================================================
ALTER PROCEDURE [dbo].[prcRptRelatorioPecas]
	@p_Status varchar = null
AS
 
BEGIN

	-- Declaração de Variáveis
	DECLARE @cdsErrorMessage	NVARCHAR(4000),
			@nidErrorSeverity	INT,
			@nidErrorState		INT

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SET FMTONLY OFF;


	BEGIN TRY

		select CD_PECA, DS_PECA, 
		case when FL_ATIVO_PECA = 'S'
				then 'Ativo'
			 when FL_ATIVO_PECA = 'N'
				then 'Inativo'
		end as Ativo
		from TB_PECA with(nolock)
		where FL_ATIVO_PECA = @p_Status or @p_Status is null
		order by DS_PECA asc

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

