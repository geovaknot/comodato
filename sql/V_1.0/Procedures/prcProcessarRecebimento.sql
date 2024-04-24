GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		André Farinelli
-- Create date: 17/11/2018
-- Description:	Update das QTD_RECEBIDA na tabela
--               TB_PEDIDO_PECA
-- =============================================
ALTER PROCEDURE [dbo].[prcProcessarRecebimento]
	@p_ID_PEDIDO				NUMERIC(9,0)	= NULL,
	@p_LotePecas				NVARCHAR(MAX)	= NULL,
	@p_nidUsuarioAtualizacao	NUMERIC(18,0)	= NULL
AS
BEGIN

	-- Declaração de Variáveis
	DECLARE @cdsErrorMessage		NVARCHAR(4000),
			@nidErrorSeverity		INT,
			@nidErrorState			INT,
			@nidLog					NUMERIC(18,0),
			@CD_PECA				VARCHAR(15), 
			@QTD_APROVADA			NUMERIC(15,3), 
			@QTD_ULTIMO_RECEBIMENTO NUMERIC(18,0),
			@STATUS_ITEM_RECEBIDO   TINYINT = 5


	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	BEGIN TRY
	
		DECLARE C1 CURSOR LOCAL FORWARD_ONLY FAST_FORWARD FOR
		SELECT dbo.TB_PEDIDO_PECA.CD_PECA,
			   dbo.TB_PEDIDO_PECA.QTD_APROVADA,
			   dbo.TB_PEDIDO_PECA.QTD_ULTIMO_RECEBIMENTO
		  FROM dbo.TB_PEDIDO_PECA 
		 INNER JOIN dbo.TB_PEDIDO
			ON dbo.TB_PEDIDO_PECA.ID_PEDIDO = dbo.TB_PEDIDO.ID_PEDIDO
		 WHERE dbo.TB_PEDIDO_PECA.ID_PEDIDO = @p_ID_PEDIDO
		   AND QTD_APROVADA > 0
		   AND dbo.TB_PEDIDO_PECA.CD_PECA IN (SELECT cdsString FROM dbo.fncGetValuesByString(@p_LotePecas, ','))
		   AND ST_STATUS_ITEM = @STATUS_ITEM_RECEBIDO
		
		OPEN C1
		FETCH NEXT FROM C1
			INTO @CD_PECA, @QTD_APROVADA, @QTD_ULTIMO_RECEBIMENTO

		WHILE @@FETCH_STATUS = 0
		BEGIN 
		
			IF (@QTD_ULTIMO_RECEBIMENTO IS NULL)
			BEGIN 
				UPDATE TB_PEDIDO_PECA
				   SET QTD_ULTIMO_RECEBIMENTO = @QTD_APROVADA
				 WHERE ID_PEDIDO = @p_ID_PEDIDO
				   AND CD_PECA = @CD_PECA
			END		

			FETCH NEXT FROM C1
				INTO @CD_PECA, @QTD_APROVADA, @QTD_ULTIMO_RECEBIMENTO
		END
		
		CLOSE C1;
		DEALLOCATE C1;
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
