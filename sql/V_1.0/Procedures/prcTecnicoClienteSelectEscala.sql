GO
/****** Object:  StoredProcedure [dbo].[prcTecnicoClienteSelectEscala]    Script Date: 08/11/2021 15:58:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Alex Natalino
-- Create date: 12/03/2018
-- Description:	Seleção de dados na tabela 
--              TB_Tecnico_Cliente
-- =============================================
ALTER PROCEDURE [dbo].[prcTecnicoClienteSelectEscala]
	@p_CD_Cliente					INT				= NULL--,	
	--@p_CD_Tecnico					VARCHAR(06)		= NULL,
	--@p_CD_Ordem						INT				= NULL
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
			CASE  ISNULL(dbo.TB_TECNICO_CLIENTE.CD_ORDEM, 0)
			WHEN 0
				THEN ROW_NUMBER() OVER(ORDER BY dbo.TB_TECNICO_CLIENTE.CD_ORDEM ASC)
			ELSE
				dbo.TB_TECNICO_CLIENTE.CD_ORDEM
			END AS CD_ORDEM,
			dbo.TB_Empresa.CD_Empresa,
			dbo.TB_Empresa.Nm_Empresa,
			dbo.TB_TECNICO.CD_TECNICO,
			dbo.TB_TECNICO.NM_TECNICO,
			ISNULL((SELECT SUM(ISNULL(C.QT_PERIODO, 0))
				FROM dbo.TB_CLIENTE AS C
				INNER JOIN dbo.TB_TECNICO_CLIENTE AS TC
					ON C.CD_CLIENTE = TC.CD_CLIENTE
				WHERE --TC.CD_CLIENTE = dbo.TB_TECNICO_CLIENTE.CD_CLIENTE
				/*AND*/ TC.CD_TECNICO = dbo.TB_TECNICO_CLIENTE.CD_TECNICO
				AND TC.CD_ORDEM = 1 
			), 0) AS nvlCargaTecnica
		FROM dbo.TB_TECNICO_CLIENTE
		INNER JOIN dbo.TB_TECNICO
			ON dbo.TB_TECNICO_CLIENTE.CD_TECNICO = dbo.TB_TECNICO.CD_TECNICO
			AND dbo.TB_TECNICO.FL_ATIVO = 'S'
		LEFT JOIN dbo.TB_Empresa
			ON dbo.TB_TECNICO.CD_EMPRESA = dbo.TB_Empresa.CD_Empresa
		WHERE dbo.TB_TECNICO_CLIENTE.CD_CLIENTE = @p_CD_Cliente
		--AND	  (	dbo.TB_TECNICO_CLIENTE.CD_TECNICO	LIKE @p_CD_Tecnico		OR @p_CD_Tecnico	IS NULL )
		--AND   ( dbo.TB_TECNICO_CLIENTE.CD_ORDEM		= @p_CD_Ordem			OR @p_CD_Ordem		IS NULL )
		
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


