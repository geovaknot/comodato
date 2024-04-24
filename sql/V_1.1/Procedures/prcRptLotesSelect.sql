GO
/****** Object:  StoredProcedure [dbo].[prcRptLotesSelect]    Script Date: 12/01/2022 11:29:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[prcRptLotesSelect]

		@p_ID_PEDIDO		INT = null,
		@p_ID_LOTE_PEDIDO	INT = NULL

AS
BEGIN

	-- Declaração de Variáveis
	DECLARE @cdsErrorMessage		NVARCHAR(4000),
			@nidErrorSeverity		INT,
			@nidErrorState			INT


	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SET FMTONLY OFF;

	BEGIN TRY

		SELECT 
			TB_PEDIDO.CD_PEDIDO,
			TB_PEDIDO.DT_CRIACAO,
			case 
				when tbLoteAprovacao.ID_LOTE_APROVACAO is null
					then 0
				when tbLoteAprovacao.ID_LOTE_APROVACAO is not null
					then tbLoteAprovacao.ID_LOTE_APROVACAO
			end as ID_LOTE_APROVACAO,
			case 
				when (tbLoteAprovacao.DT_APROVACAO is null)
					then getdate()
				when (tbLoteAprovacao.DT_APROVACAO is not null)
					then tbLoteAprovacao.DT_APROVACAO
			end as DT_APROVACAO,
			tbStatusPedido.DS_STATUS_PEDIDO,
			TB_PEDIDO.TP_TIPO_PEDIDO,

			'(' + TB_TECNICO.CD_TECNICO + ') ' + TB_TECNICO.NM_TECNICO AS DS_TECNICO,
			TB_EMPRESA.Nm_Empresa,
			'(' + Convert(VARCHAR, TB_CLIENTE.CD_CLIENTE) + ') ' + TB_CLIENTE.NM_CLIENTE AS DS_CLIENTE,
			
			TB_PEDIDO_PECA.CD_PECA,
			case 
				when tbPecaRecuperada.CD_PECA_RECUPERADA is null
					then 0
				when tbPecaRecuperada.CD_PECA_RECUPERADA is not null
					then tbPecaRecuperada.CD_PECA_RECUPERADA
			end as CD_PECA_RECUPERADA,
			TB_PECA.DS_PECA,
			TB_PEDIDO_PECA.QTD_SOLICITADA,
			TB_PEDIDO_PECA.QTD_APROVADA,
			case 
				when (TB_PEDIDO_PECA.QTD_APROVADA_3M1 is null and TB_PEDIDO_PECA.QTD_APROVADA_3M2 is null)
					then TB_PEDIDO_PECA.QTD_APROVADA 
				when (TB_PEDIDO_PECA.QTD_APROVADA_3M1 is not null)
					then TB_PEDIDO_PECA.QTD_APROVADA_3M1
			end as QTD_APROVADA_3M1,
			
			TB_PEDIDO_PECA.QTD_APROVADA_3M2,
			TB_PEDIDO_PECA.QTD_RECEBIDA,
			TB_PEDIDO_PECA.ST_STATUS_ITEM,
			TB_PEDIDO_PECA.VL_PECA,
			TB_PEDIDO.ID_PEDIDO
		FROM
			TB_PEDIDO
			INNER JOIN TB_PEDIDO_PECA ON TB_PEDIDO.ID_PEDIDO = TB_PEDIDO_PECA.ID_PEDIDO
			left JOIN tbLoteAprovacao ON TB_PEDIDO_PECA.ID_LOTE_APROVACAO = tbLoteAprovacao.ID_LOTE_APROVACAO
			INNER JOIN tbStatusPedido ON TB_PEDIDO.ID_STATUS_PEDIDO = tbStatusPedido.ID_STATUS_PEDIDO
			LEFT JOIN TB_TECNICO ON TB_PEDIDO.CD_TECNICO = TB_TECNICO.CD_TECNICO
			LEFT JOIN TB_EMPRESA ON TB_TECNICO.CD_EMPRESA = TB_EMPRESA.CD_Empresa
			LEFT JOIN TB_CLIENTE ON TB_PEDIDO.CD_CLIENTE = TB_CLIENTE.CD_CLIENTE
			LEFT JOIN TB_PECA ON TB_PEDIDO_PECA.CD_PECA = TB_PECA.CD_PECA
			LEFT JOIN tbPecaRecuperada ON TB_PEDIDO_PECA.CD_PECA = tbPecaRecuperada.CD_PECA
		WHERE 
			TB_PEDIDO.ID_PEDIDO = @p_ID_PEDIDO
			AND (TB_PEDIDO_PECA.ID_LOTE_APROVACAO = @p_ID_LOTE_PEDIDO or @p_ID_LOTE_PEDIDO IS NULL)
		ORDER BY
			TB_PECA.DS_PECA
		
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


--select * from TB_PEDIDO where CD_PEDIDO = 22
--select * from TB_PEDIDO_PECA where ID_PEDIDO = 22