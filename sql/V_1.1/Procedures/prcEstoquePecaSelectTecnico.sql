GO
/****** Object:  StoredProcedure [dbo].[prcEstoquePecaSelectTecnico]    Script Date: 25/01/2022 15:50:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Flavio Ribeiro
-- Create date: 
-- Description:	Seleção de dados na tabela tbEstoque com tbEstoquePeca
-- forçando relacionamento com tbUsuario via tbEstoque.ID_USU_RESPONSAVEL
-- através do campo tbUsuario.CD_TECNICO
-- =============================================
ALTER PROCEDURE [dbo].[prcEstoquePecaSelectTecnico]
	@p_CD_TECNICO			VARCHAR(06) = NULL,
	@p_CD_PECA				VARCHAR(15)	= NULL,
	@p_CD_GRUPO_MODELO		VARCHAR(15)	= NULL
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

		SELECT	dbo.TB_TECNICO.ID_USUARIO,
				dbo.tbEstoque.*,
				dbo.tbEstoquePeca.ID_ESTOQUE_PECA,
				dbo.tbEstoquePeca.CD_PECA,
				dbo.TB_PECA.DS_PECA, 
				dbo.TB_PECA.TX_UNIDADE,
				dbo.tbEstoquePeca.QT_PECA_ATUAL,
				dbo.tbEstoquePeca.QT_PECA_MIN,
				dbo.tbEstoquePeca.DT_ULT_MOVIM,
				(SELECT MAX(dbo.tbEstoqueMovi.DT_MOVIMENTACAO) 
					FROM dbo.tbEstoqueMovi (nolock) 
					WHERE dbo.tbEstoqueMovi.CD_PECA				= dbo.tbEstoquePeca.CD_PECA 
					AND dbo.tbEstoqueMovi.ID_ESTOQUE			= dbo.tbEstoquePeca.ID_ESTOQUE 
					AND dbo.tbEstoqueMovi.CD_TP_MOVIMENTACAO	= 4 -- Ajuste de Saida de Estoque
				) AS DT_MOVIMENTACAO_AJUSTE_SAIDA,
				(select COALESCE(sum(COALESCE(ped_item.QTD_APROVADA, 0)) - sum(COALESCE(ped_item.QTD_RECEBIDA, 0)), 0)
 				   from TB_PEDIDO pedido
				  inner join TB_PEDIDO_PECA ped_item
					 on pedido.ID_PEDIDO = ped_item.ID_PEDIDO
			      where pedido.CD_TECNICO = dbo.tbEstoque.CD_TECNICO
				    and pedido.ID_STATUS_PEDIDO in(3, 5, 6) --Aprovado / Pendencia / Recebido com Pendência
				    and ped_item.CD_PECA = dbo.tbEstoquePeca.CD_PECA
				    and ped_item.ST_STATUS_ITEM = 3 --Aprovado
				) AS QTD_REC_NAO_APROV
		FROM dbo.TB_TECNICO (nolock)
			INNER JOIN dbo.tbEstoque
			ON dbo.TB_TECNICO.ID_USUARIO = dbo.tbEstoque.ID_USU_RESPONSAVEL
			AND dbo.tbEstoque.FL_ATIVO = 'S'
			INNER JOIN dbo.tbEstoquePeca (nolock)
			ON dbo.tbEstoque.ID_ESTOQUE = dbo.tbEstoquePeca.ID_ESTOQUE
			INNER JOIN dbo.TB_PECA (nolock)
			ON dbo.tbEstoquePeca.CD_PECA = dbo.TB_PECA.CD_PECA
			INNER JOIN dbo.tbPlanoZero (nolock)
			ON dbo.TB_PECA.CD_PECA = dbo.tbPlanoZero.CD_PECA
			AND dbo.tbPlanoZero.CD_GRUPO_MODELO = @p_CD_GRUPO_MODELO
		WHERE	dbo.TB_TECNICO.CD_TECNICO		= @p_CD_TECNICO -- através do técnico vincula os ID_USUARIOS no primeiro JOIN  
		AND		dbo.tbEstoque.TP_ESTOQUE_TEC_3M = 'TEC'			-- forçar a consulta do estoque somente do TÉCNICO
		AND	  ( dbo.TB_PECA.CD_PECA				= @p_CD_PECA	OR @p_CD_PECA IS NULL)

		--Teste André - Remover peças c qtd zero:
		--AND	(dbo.tbEstoquePeca.QT_PECA_ATUAL > 0)
		ORDER BY 
			dbo.TB_PECA.DS_PECA

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


--select * from tbPlanoZero where cd_grupo_modelo <> '700a'
--select * from TB_ATIVO_FIXO where cd_ativo_fixo = '134994'


