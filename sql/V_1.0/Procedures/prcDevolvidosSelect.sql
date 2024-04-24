GO
/****** Object:  StoredProcedure [dbo].[prcDevolvidosSelect]    Script Date: 28/09/2021 18:02:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Andre Farinelli
-- Create date: 08/07/2018
-- Description:	Seleção de dados de equipamentos
--				devolvidos. 
-- =============================================

ALTER PROCEDURE [dbo].[prcDevolvidosSelect]
		@p_DT_DEV_INICIAL		DATETIME,
		@p_DT_DEV_FINAL			DATETIME,
		@p_CD_CLIENTE			NUMERIC(6,0) = NULL,
		@p_CD_VENDEDOR			NUMERIC(6,0) = NULL,
		@p_CD_GRUPO				VARCHAR(10)	= NULL,
		@p_CD_MOTIVO_DEVOLUCAO	VARCHAR(1)	= NULL
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
			TB_ATIVO_CLIENTE.DT_DEVOLUCAO,
			TB_ATIVO_CLIENTE.CD_ATIVO_FIXO,
			TB_ATIVO_CLIENTE.CD_ATIVO_FIXO + ' - ' + COALESCE(DS_MODELO,'') + ' - ' + COALESCE(TX_ANO_MÁQUINA,'') AS DS_ATIVO_FIXO,
			TB_ATIVO_CLIENTE.ID_ATIVO_CLIENTE, 
			TB_CLIENTE.CD_CLIENTE,
			TB_CLIENTE.NM_CLIENTE + ' - (' + CONVERT(VARCHAR(6),COALESCE(TB_CLIENTE.CD_CLIENTE,'')) + ') - ' + COALESCE(TB_CLIENTE.EN_CIDADE,'') + ' - ' + COALESCE(TB_CLIENTE.EN_ESTADO,'') AS NM_CLIENTE,
			TB_ATIVO_CLIENTE.CD_MOTIVO_DEVOLUCAO,
			TB_MOTIVO_DEVOLUCAO.DS_MOTIVO_DEVOLUCAO ,
			TB_ATIVO_CLIENTE.DT_NOTAFISCAL,
			TB_ATIVO_CLIENTE.NR_NOTAFISCAL,
			TB_MODELO.DS_MODELO,
			TB_LINHA_PRODUTO.DS_LINHA_PRODUTO,
			TB_DEPRECIACAO.VL_CUSTO_ATIVO + TB_DEPRECIACAO.VL_DEPREC_TOTAL as VL_RESIDUAL,
			TB_VENDEDOR.NM_VENDEDOR,
			TB_GRUPO.DS_GRUPO
		FROM TB_ATIVO_CLIENTE 
			INNER JOIN TB_ATIVO_FIXO       
			ON TB_ATIVO_FIXO.CD_ATIVO_FIXO = TB_ATIVO_CLIENTE.CD_ATIVO_FIXO 
			INNER JOIN TB_CLIENTE 
			ON TB_CLIENTE.CD_CLIENTE = TB_ATIVO_CLIENTE.CD_CLIENTE
			LEFT JOIN TB_VENDEDOR --INNER no VB
			ON TB_CLIENTE.CD_VENDEDOR = TB_VENDEDOR.CD_VENDEDOR
			LEFT JOIN TB_GRUPO --INNER no VB
			ON TB_CLIENTE.CD_GRUPO = TB_GRUPO.CD_GRUPO
			LEFT JOIN TB_LINHA_PRODUTO --INNER no VB
			ON TB_ATIVO_FIXO.CD_LINHA_PRODUTO = TB_LINHA_PRODUTO.CD_LINHA_PRODUTO 
			LEFT JOIN TB_MODELO --INNER no VB
			ON TB_ATIVO_FIXO.CD_MODELO = TB_MODELO.CD_MODELO
			LEFT JOIN TB_MOTIVO_DEVOLUCAO --INNER no VB
			ON TB_ATIVO_CLIENTE.CD_MOTIVO_DEVOLUCAO = TB_MOTIVO_DEVOLUCAO.CD_MOTIVO_DEVOLUCAO
			--LEFT JOIN TB_SITUACAO_ATIVO   
			--ON TB_ATIVO_FIXO.CD_SITUACAO_ATIVO = TB_SITUACAO_ATIVO.CD_SITUACAO_ATIVO
			--LEFT JOIN TB_STATUS_ATIVO     
			--ON TB_ATIVO_FIXO.CD_STATUS_ATIVO = TB_STATUS_ATIVO.CD_STATUS_ATIVO
			LEFT JOIN TB_DEPRECIACAO
			ON TB_ATIVO_FIXO.CD_ATIVO_FIXO = TB_DEPRECIACAO.CD_ATIVO_FIXO
		WHERE (TB_ATIVO_CLIENTE.DT_DEVOLUCAO BETWEEN @p_DT_DEV_INICIAL AND @p_DT_DEV_FINAL)
		AND (TB_CLIENTE.CD_CLIENTE			= @p_CD_CLIENTE		OR @p_CD_CLIENTE IS NULL)
		AND (TB_VENDEDOR.CD_VENDEDOR	= @p_CD_VENDEDOR	OR @p_CD_VENDEDOR IS NULL)
		--AND (TB_VENDEDOR.FL_ATIVO = 'S')
		AND (TB_GRUPO.CD_GRUPO	= @p_CD_GRUPO	OR @p_CD_GRUPO IS NULL)
		AND (TB_ATIVO_CLIENTE.CD_MOTIVO_DEVOLUCAO	= @p_CD_MOTIVO_DEVOLUCAO	OR @p_CD_MOTIVO_DEVOLUCAO IS NULL)

		--AND dbo.TB_ATIVO_CLIENTE.CD_MOTIVO_DEVOLUCAO <> 'T'
		--ORDER BY
			--TB_ATIVO_CLIENTE.DT_DEVOLUCAO DESC
		
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
