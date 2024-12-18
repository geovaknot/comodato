GO
/****** Object:  StoredProcedure [dbo].[prcEstoquePecaIntermediarioSelect]    Script Date: 14/09/2021 11:53:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Flavio Ribeiro
-- Create date: 
-- Description:	Consulta de estoque de Peças (Estoque Intermediário)
-- =============================================
ALTER PROCEDURE [dbo].[prcEstoquePecaIntermediarioSelect]
	@p_CD_TECNICO		VARCHAR(6) = NULL
	,@p_ID_ESTOQUE		BIGINT = NULL
	,@p_CD_PECA			VARCHAR(15) = NULL
	,@p_FL_ATIVO_PECA	VARCHAR(1) = 'S'
	,@p_nidUsuario		BIGINT = NULL
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

	IF (@p_CD_TECNICO IS NOT NULL)
	BEGIN

	SELECT	 TB_PECA.CD_PECA
			,TB_PECA.DS_PECA
			,TB_PECA.TX_UNIDADE
			,SUM(tbEstoquePeca.QT_PECA_ATUAL) AS QTD_PECA,
			--,TB_PECA.FL_ATIVO_PECA
			--,ISNULL(TB_TECNICO.NM_TECNICO, tbEstoque.CD_ESTOQUE) ESTOQUE
			(
			  select COALESCE(sum(COALESCE(ped_item.QTD_APROVADA, 0)) - sum(COALESCE(ped_item.QTD_RECEBIDA, 0)), 0)
				from TB_PEDIDO pedido
			   inner join TB_PEDIDO_PECA ped_item
				  on pedido.ID_PEDIDO = ped_item.ID_PEDIDO
			   where pedido.CD_TECNICO = @p_CD_TECNICO
				 and pedido.ID_STATUS_PEDIDO in(3, 5, 6) --Aprovado / Pendencia / Recebido com Pendência
				 and ped_item.CD_PECA = TB_PECA.CD_PECA
				 and ped_item.ST_STATUS_ITEM in(3, 5, 7) --Aprovado
     	   ) AS QTD_REC_NAO_APROV

		FROM TB_PECA(nolock)
		inner join tbEstoquePeca(nolock)
			on tbEstoquePeca.CD_PECA = TB_PECA.CD_PECA
		inner join tbEstoque(nolock)
			on tbEstoque.ID_ESTOQUE = tbEstoquePeca.ID_ESTOQUE
			AND		(@p_nidUsuario IS NULL OR tbEstoque.ID_USU_RESPONSAVEL IN (SELECT nidUsuario From fncRetornaUsuariosAcesso( @p_nidUsuario )))

		left join TB_TECNICO(nolock)
			on TB_TECNICO.CD_TECNICO = tbEstoque.CD_TECNICO

		WHERE	(tbEstoque.ID_ESTOQUE = @p_ID_ESTOQUE OR @p_ID_ESTOQUE IS NULL)
		AND		(TB_PECA.CD_PECA = @p_CD_PECA OR @p_CD_PECA IS NULL)
		AND		TB_PECA.FL_ATIVO_PECA = @p_FL_ATIVO_PECA

		AND (TB_TECNICO.CD_TECNICO = @p_CD_TECNICO)
		AND	(dbo.tbEstoquePeca.QT_PECA_ATUAL > 0)

		GROUP BY  TB_PECA.CD_PECA
				,TB_PECA.DS_PECA
				,TB_PECA.TX_UNIDADE
				--,TB_PECA.VL_PECA
				--,TB_PECA.FL_ATIVO_PECA
				--,ISNULL(TB_TECNICO.NM_TECNICO, tbEstoque.CD_ESTOQUE) 

	END
	ELSE
	BEGIN

		SELECT	 TB_PECA.CD_PECA,
				 TB_PECA.DS_PECA
				,TB_PECA.TX_UNIDADE
				,TB_PECA.VL_PECA
				,MAX(tbEstoquePeca.DT_ULT_MOVIM) DT_ULT_MOVIM
				,SUM(tbEstoquePeca.QT_PECA_ATUAL) QT_PECA_ATUAL
				,TB_PECA.FL_ATIVO_PECA
				,ISNULL(TB_TECNICO.NM_TECNICO, tbEstoque.CD_ESTOQUE) ESTOQUE,
				(
				  select COALESCE(sum(COALESCE(ped_item.QTD_APROVADA, 0)) - sum(COALESCE(ped_item.QTD_RECEBIDA, 0)), 0) 
					from TB_PEDIDO pedido
				   inner join TB_PEDIDO_PECA ped_item
					  on pedido.ID_PEDIDO = ped_item.ID_PEDIDO
				   where pedido.CD_TECNICO = MAX(TB_TECNICO.CD_TECNICO)
					 and pedido.ID_STATUS_PEDIDO in(3, 5, 6) --Aprovado / Pendencia / Recebido com Pendência
					 and ped_item.CD_PECA = TB_PECA.CD_PECA
					 and ped_item.ST_STATUS_ITEM in(3, 5, 7) --Aprovado
     		   ) AS QTD_REC_NAO_APROV

		FROM TB_PECA(nolock)
		inner join tbEstoquePeca(nolock)
			on tbEstoquePeca.CD_PECA = TB_PECA.CD_PECA
		inner join tbEstoque(nolock)
			on tbEstoque.ID_ESTOQUE = tbEstoquePeca.ID_ESTOQUE
			AND		(@p_nidUsuario IS NULL OR tbEstoque.ID_USU_RESPONSAVEL IN (SELECT nidUsuario From fncRetornaUsuariosAcesso( @p_nidUsuario )))

		left join TB_TECNICO(nolock)
			on TB_TECNICO.CD_TECNICO = tbEstoque.CD_TECNICO

		WHERE	(tbEstoque.ID_ESTOQUE = @p_ID_ESTOQUE OR @p_ID_ESTOQUE IS NULL)
		AND		(TB_PECA.CD_PECA = @p_CD_PECA OR @p_CD_PECA IS NULL)
		AND		TB_PECA.FL_ATIVO_PECA = @p_FL_ATIVO_PECA

		--Teste André - Remover peças c qtd zero:
		--AND	(dbo.tbEstoquePeca.QT_PECA_ATUAL > 0)

		GROUP BY TB_PECA.CD_PECA, 
				TB_PECA.DS_PECA
				,TB_PECA.TX_UNIDADE
				,TB_PECA.VL_PECA
				,TB_PECA.FL_ATIVO_PECA
				,ISNULL(TB_TECNICO.NM_TECNICO, tbEstoque.CD_ESTOQUE) 		
		
	END

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





