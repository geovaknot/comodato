GO
/****** Object:  StoredProcedure [dbo].[prcPecaSelect]    Script Date: 17/06/2021 15:22:43 ******/
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
ALTER PROCEDURE [dbo].[prcPecaSelect]
	@p_CD_PECA							VARCHAR(15) = NULL,
	@p_DS_PECA							VARCHAR(50) = NULL,
	@p_TX_UNIDADE						VARCHAR(2) = NULL,
	@p_QTD_ESTOQUE						NUMERIC(15,3) = NULL,
	@p_QTD_MINIMA						NUMERIC(15,3) = NULL,
	@p_VL_PECA							NUMERIC(14,2) = NULL,
	@p_TP_PECA							VARCHAR(1) = NULL,
	@p_FL_ATIVO_PECA					VARCHAR(1) = NULL,
	@p_nidUsuarioAtualizacao			NUMERIC(18,0)	= NULL,
	@p_CD_TECNICO						VARCHAR(6) = NULL
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
			UPPER(CD_PECA) AS CD_PECA,
			DS_PECA,
			TX_UNIDADE,
			QTD_ESTOQUE,
			QTD_MINIMA,
			VL_PECA,
			TP_PECA,
			FL_ATIVO_PECA,
			CASE WHEN (@p_CD_TECNICO IS NOT NULL) AND (@p_CD_TECNICO <> '') THEN
			(
				select COALESCE(sum(COALESCE(ped_item.QTD_APROVADA, 0)) - sum(COALESCE(ped_item.QTD_RECEBIDA, 0)), 0)
					from TB_PEDIDO pedido
					inner join TB_PEDIDO_PECA ped_item
					on pedido.ID_PEDIDO = ped_item.ID_PEDIDO
					where pedido.CD_TECNICO = @p_CD_TECNICO 
					and pedido.ID_STATUS_PEDIDO in(3, 5, 6) --Aprovado / Pendencia / Recebido com Pendência
					and ped_item.CD_PECA = TB_PECA.CD_PECA
					and ped_item.ST_STATUS_ITEM = 3 --Aprovado
			)
			ELSE 0
			END AS QTD_REC_NAO_APROV
		FROM	TB_PECA
		WHERE (	CD_PECA			= @p_CD_PECA		OR @p_CD_PECA		IS NULL )
		AND	  (	DS_PECA			LIKE @p_DS_PECA		OR @p_DS_PECA		IS NULL )
		AND	  (	TX_UNIDADE		= @p_TX_UNIDADE		OR @p_TX_UNIDADE	IS NULL )
		AND	  (	QTD_ESTOQUE		= @p_QTD_ESTOQUE	OR @p_QTD_ESTOQUE	IS NULL )
		AND	  (	QTD_MINIMA		= @p_QTD_MINIMA		OR @p_QTD_MINIMA	IS NULL )
		AND	  (	VL_PECA			= @p_VL_PECA		OR @p_VL_PECA		IS NULL )
		AND	  (	TP_PECA			= @p_TP_PECA		OR @p_TP_PECA		IS NULL )
		AND   ( FL_ATIVO_PECA   = @p_FL_ATIVO_PECA	OR @p_FL_ATIVO_PECA IS NULL )
		ORDER BY
			DS_PECA,      
			CD_PECA			 
		
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


