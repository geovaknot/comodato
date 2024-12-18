GO
/****** Object:  StoredProcedure [dbo].[prcRelatorioLocadosSelect]    Script Date: 25/02/2023 17:53:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Andre Farinelli
-- Create date: 08/07/2018
-- Description:	Seleção de dados de equipamentos
--				locados. 
-- =============================================

ALTER PROCEDURE [dbo].[prcRelatorioLocadosSelect]
	 	@p_DT_INICIAL	DATETIME = NULL,
		@p_DT_FINAL		DATETIME = NULL,
		@p_CD_CLIENTE		NUMERIC(6,0) = NULL,
		@p_CD_VENDEDOR		NUMERIC(6,0) = NULL,
		@p_CD_GRUPO			VARCHAR(10)	= NULL
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
			atv.CD_ATIVO_FIXO,
			TB_MODELO.DS_MODELO,
			atv.DT_NOTAFISCAL,
			--QT_Vencidos
			atv.NR_NOTAFISCAL,
			TB_TIPO.DS_TIPO,
			atv.CD_CLIENTE,
			TB_CLIENTE.NM_CLIENTE,
			atv.VL_ALUGUEL,
			atv.TX_TERMOPGTO,
			--TB_DadosPagamento.NR_NotaFiscal,
			--TB_DadosPagamento.DT_Solicitacao
			(select top 1 pg.NR_NotaFiscal from TB_DadosPagamento pg inner join TB_DadosFaturamento fat
				on pg.ID_DADOS_FATURAMENTO = fat.ID and fat.AtivoFixo = atv.CD_ATIVO_FIXO
				--inner join TB_ATIVO_CLIENTE on fat.AtivoFixo = atv.CD_ATIVO_FIXO
				where pg.DataEmissaoNF >= @p_DT_INICIAL and pg.DataEmissaoNF <= @p_DT_FINAL) as Venc_1,
			(select top 1 pg.DataEmissaoNF from TB_DadosPagamento pg inner join TB_DadosFaturamento fat
				on pg.ID_DADOS_FATURAMENTO = fat.ID and fat.AtivoFixo = atv.CD_ATIVO_FIXO
				--inner join TB_ATIVO_CLIENTE on fat.AtivoFixo = atv.CD_ATIVO_FIXO
				where pg.DataEmissaoNF >= @p_DT_INICIAL and pg.DataEmissaoNF <= @p_DT_FINAL) as DT_NF1,
			(select top 1 fat.AluguelApos3anos from TB_DadosPagamento pg inner join TB_DadosFaturamento fat
				on pg.ID_DADOS_FATURAMENTO = fat.ID and fat.AtivoFixo = atv.CD_ATIVO_FIXO
				--inner join TB_ATIVO_CLIENTE on fat.AtivoFixo = atv.CD_ATIVO_FIXO
				where pg.DataEmissaoNF >= @p_DT_INICIAL and pg.DataEmissaoNF <= @p_DT_FINAL) as VL_NF1,

			(select pg.NR_NotaFiscal from TB_DadosPagamento pg inner join TB_DadosFaturamento fat
				on pg.ID_DADOS_FATURAMENTO = fat.ID and fat.AtivoFixo = atv.CD_ATIVO_FIXO
				--inner join TB_ATIVO_CLIENTE on fat.AtivoFixo = atv.CD_ATIVO_FIXO
				--where (pg.DataEmissaoNF >= DATEADD(DAY, 90, (select top 1 pg.DataEmissaoNF from TB_DadosPagamento pg inner join TB_DadosFaturamento fat
				--on pg.ID_DADOS_FATURAMENTO = fat.ID and fat.AtivoFixo = atv.CD_ATIVO_FIXO))) 
				order by pg.DataEmissaoNF OFFSET 1 ROWS
					FETCH NEXT 1 ROWS ONLY) as Venc_2,
			(select pg.DataEmissaoNF from TB_DadosPagamento pg inner join TB_DadosFaturamento fat
				on pg.ID_DADOS_FATURAMENTO = fat.ID and fat.AtivoFixo = atv.CD_ATIVO_FIXO
				--inner join TB_ATIVO_CLIENTE on fat.AtivoFixo = atv.CD_ATIVO_FIXO
				--where (pg.DataEmissaoNF >= DATEADD(DAY, 90, (select top 1 pg.DataEmissaoNF from TB_DadosPagamento pg inner join TB_DadosFaturamento fat
				--on pg.ID_DADOS_FATURAMENTO = fat.ID and fat.AtivoFixo = atv.CD_ATIVO_FIXO))) 
				order by pg.DataEmissaoNF OFFSET 1 ROWS
				FETCH NEXT 1 ROWS ONLY ) as DT_NF2,
			(select fat.AluguelApos3anos from TB_DadosPagamento pg inner join TB_DadosFaturamento fat
				on pg.ID_DADOS_FATURAMENTO = fat.ID
				inner join TB_ATIVO_CLIENTE on fat.AtivoFixo = atv.CD_ATIVO_FIXO
				--where (pg.DataEmissaoNF >= DATEADD(DAY, 90, (select top 1 pg.DataEmissaoNF from TB_DadosPagamento pg inner join TB_DadosFaturamento fat
				--on pg.ID_DADOS_FATURAMENTO = fat.ID and fat.AtivoFixo = atv.CD_ATIVO_FIXO))) 
				order by pg.DataEmissaoNF OFFSET 1 ROWS
				FETCH NEXT 1 ROWS ONLY) as VL_NF2,

			(select pg.NR_NotaFiscal from TB_DadosPagamento pg inner join TB_DadosFaturamento fat
				on pg.ID_DADOS_FATURAMENTO = fat.ID and fat.AtivoFixo = atv.CD_ATIVO_FIXO
				--inner join TB_ATIVO_CLIENTE on fat.AtivoFixo = atv.CD_ATIVO_FIXO
				--where (pg.DataEmissaoNF >= DATEADD(DAY, 180, (select top 1 pg.DataEmissaoNF from TB_DadosPagamento pg inner join TB_DadosFaturamento fat
				--on pg.ID_DADOS_FATURAMENTO = fat.ID and fat.AtivoFixo = atv.CD_ATIVO_FIXO order by pg.DataEmissaoNF ))) 
				order by pg.DataEmissaoNF OFFSET 2 ROWS
				FETCH NEXT 1 ROWS ONLY) as Venc_3,
			(select pg.DataEmissaoNF from TB_DadosPagamento pg inner join TB_DadosFaturamento fat
				on pg.ID_DADOS_FATURAMENTO = fat.ID and fat.AtivoFixo = atv.CD_ATIVO_FIXO
				--inner join TB_ATIVO_CLIENTE on fat.AtivoFixo = atv.CD_ATIVO_FIXO
				--where (pg.DataEmissaoNF >= DATEADD(DAY, 90, (select top 1 pg.DataEmissaoNF from TB_DadosPagamento pg inner join TB_DadosFaturamento fat
				--on pg.ID_DADOS_FATURAMENTO = fat.ID and fat.AtivoFixo = atv.CD_ATIVO_FIXO order by pg.DataEmissaoNF ))) 
				order by pg.DataEmissaoNF OFFSET 2 ROWS
				FETCH NEXT 1 ROWS ONLY) as DT_NF3,
			(select fat.AluguelApos3anos from TB_DadosPagamento pg inner join TB_DadosFaturamento fat
				on pg.ID_DADOS_FATURAMENTO = fat.ID and fat.AtivoFixo = atv.CD_ATIVO_FIXO
				--inner join TB_ATIVO_CLIENTE on fat.AtivoFixo = atv.CD_ATIVO_FIXO
				--where (pg.DataEmissaoNF >= DATEADD(DAY, 180, (select top 1 pg.DataEmissaoNF from TB_DadosPagamento pg inner join TB_DadosFaturamento fat
				--on pg.ID_DADOS_FATURAMENTO = fat.ID and fat.AtivoFixo = atv.CD_ATIVO_FIXO order by pg.DataEmissaoNF ))) 
				order by pg.DataEmissaoNF OFFSET 2 ROWS
				FETCH NEXT 1 ROWS ONLY) as VL_NF3,

			(select pg.NR_NotaFiscal from TB_DadosPagamento pg inner join TB_DadosFaturamento fat
				on pg.ID_DADOS_FATURAMENTO = fat.ID and fat.AtivoFixo = atv.CD_ATIVO_FIXO
				--inner join TB_ATIVO_CLIENTE on fat.AtivoFixo = atv.CD_ATIVO_FIXO
				--where (pg.DataEmissaoNF >= DATEADD(DAY, 270, (select top 1 pg.DataEmissaoNF from TB_DadosPagamento pg inner join TB_DadosFaturamento fat
				--on pg.ID_DADOS_FATURAMENTO = fat.ID and fat.AtivoFixo = atv.CD_ATIVO_FIXO))) 
				order by pg.DataEmissaoNF OFFSET 3 ROWS
				FETCH NEXT 1 ROWS ONLY) as Venc_4,
			(select pg.DataEmissaoNF from TB_DadosPagamento pg inner join TB_DadosFaturamento fat
				on pg.ID_DADOS_FATURAMENTO = fat.ID and fat.AtivoFixo = atv.CD_ATIVO_FIXO
				--inner join TB_ATIVO_CLIENTE on fat.AtivoFixo = atv.CD_ATIVO_FIXO
				--where (pg.DataEmissaoNF >= DATEADD(DAY, 270, (select top 1 pg.DataEmissaoNF from TB_DadosPagamento pg inner join TB_DadosFaturamento fat
				--on pg.ID_DADOS_FATURAMENTO = fat.ID and fat.AtivoFixo = atv.CD_ATIVO_FIXO))) 
				order by pg.DataEmissaoNF OFFSET 3 ROWS
				FETCH NEXT 1 ROWS ONLY) as DT_NF4,
			(select fat.AluguelApos3anos from TB_DadosPagamento pg inner join TB_DadosFaturamento fat
				on pg.ID_DADOS_FATURAMENTO = fat.ID and fat.AtivoFixo = atv.CD_ATIVO_FIXO
				--inner join TB_ATIVO_CLIENTE on fat.AtivoFixo = atv.CD_ATIVO_FIXO
				--where (pg.DataEmissaoNF >= DATEADD(DAY, 270, (select top 1 pg.DataEmissaoNF from TB_DadosPagamento pg inner join TB_DadosFaturamento fat
				--on pg.ID_DADOS_FATURAMENTO = fat.ID and fat.AtivoFixo = atv.CD_ATIVO_FIXO))) 
				order by pg.DataEmissaoNF OFFSET 3 ROWS
				FETCH NEXT 1 ROWS ONLY) as VL_NF4
			--Venc_3
			--Venc_4
			--TB_VENDEDOR.NM_VENDEDOR,
			--TB_GRUPO.DS_GRUPO
		FROM TB_ATIVO_CLIENTE atv
			INNER JOIN TB_ATIVO_FIXO       
			ON atv.CD_ATIVO_FIXO = TB_ATIVO_FIXO.CD_ATIVO_FIXO
			INNER JOIN TB_CLIENTE 
			ON TB_CLIENTE.CD_CLIENTE = atv.CD_CLIENTE
			LEFT JOIN TB_VENDEDOR
			ON TB_CLIENTE.CD_VENDEDOR = TB_VENDEDOR.CD_VENDEDOR
			LEFT JOIN TB_GRUPO
			ON TB_CLIENTE.CD_GRUPO = TB_GRUPO.CD_GRUPO
			LEFT JOIN TB_MODELO
			ON TB_ATIVO_FIXO.CD_MODELO = TB_MODELO.CD_MODELO
			LEFT JOIN TB_TIPO
			ON atv.CD_TIPO = TB_TIPO.CD_TIPO
			--LEFT JOIN TB_DadosFaturamento
			--ON TB_ATIVO_CLIENTE.ID_ATIVO_CLIENTE = TB_DadosFaturamento.ID_ATIVO_CLIENTE
			--LEFT JOIN TB_DadosPagamento
			--ON TB_DadosFaturamento.ID = TB_DadosPagamento.ID_DADOS_FATURAMENTO
		WHERE (atv.DT_NOTAFISCAL BETWEEN @p_DT_INICIAL AND @p_DT_FINAL)
		AND (TB_CLIENTE.CD_CLIENTE			= @p_CD_CLIENTE		OR @p_CD_CLIENTE IS NULL)
		AND (TB_VENDEDOR.CD_VENDEDOR	= @p_CD_VENDEDOR	OR @p_CD_VENDEDOR IS NULL)
		AND (TB_GRUPO.CD_GRUPO	= @p_CD_GRUPO	OR @p_CD_GRUPO IS NULL)
		AND (TB_TIPO.DS_TIPO = 'LOCACAO')
		AND (atv.DT_DEVOLUCAO is null or atv.DT_DEVOLUCAO < '1950-01-01')
		ORDER BY TB_CLIENTE.NM_CLIENTE,atv.DT_NOTAFISCAL
		
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




--select fat.AluguelApos3anos, pag.DataEmissaoNF, pag.NR_NotaFiscal  from TB_DadosFaturamento fat
--inner join TB_DadosPagamento pag on fat.ID = pag.ID_DADOS_FATURAMENTO
--where fat.AtivoFixo = 'E05654' order by pag.DataEmissaoNF OFFSET 1 ROWS
--				FETCH NEXT 1 ROWS ONLY

--select * from TB_DadosPagamento where ID_DADOS_FATURAMENTO in (2060
--,598
--,1283)