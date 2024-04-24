GO
/****** Object:  StoredProcedure [dbo].[prcEstoqueMoviInsert]    Script Date: 05/09/2022 16:12:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Alex natalino
-- Create date: 21/03/2018
-- Description:	Inclusão de Dados na Tabela de
--              tbEstoqueMovi
-- =============================================
ALTER PROCEDURE [dbo].[prcEstoqueMoviInsert]	
	@p_CD_TP_MOVIMENTACAO		CHAR(1),
	@p_ID_OS					BIGINT			= NULL,
	@p_DT_MOVIMENTACAO			DATETIME,
	@p_ID_ESTOQUE				BIGINT			= NULL,
	@p_CD_PECA					VARCHAR(15),
	@p_QT_PECA					NUMERIC(15,3),
	@p_ID_USU_MOVI				BIGINT			= NULL,
	@p_ID_ESTOQUE_ORIGEM		BIGINT			= NULL,
	@p_TP_ENTRADA_SAIDA			CHAR(1),
	@p_CD_CLIENTE				NUMERIC(06)		= NULL,
	@p_VL_VALOR_PECA			NUMERIC(18, 2)  = NULL,
	@p_nidUsuarioAtualizacao	NUMERIC(18,0)	= NULL,
	@p_ID_ESTOQUE_MOVI			BIGINT			OUTPUT
	
AS
BEGIN
	/*
		@p_VL_VALOR_PECA -> Quando Nulo, registra o valor atual da peça
		-> [prcPedidoPecaProcessarAprovacao] >  Não é enviado o VALOR_PECA, alterar caso necessário registrar o valor da peça 
												ref. a data da solicitação.
	*/

	-- Declaração de Variáveis
	DECLARE @cdsErrorMessage	NVARCHAR(4000),
			@nidErrorSeverity	INT,
			@nidErrorState		INT,
			@nidLog				NUMERIC(18,0)
			

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY

		BEGIN TRANSACTION
		IF (@p_VL_VALOR_PECA IS NULL)
		BEGIN 
			SELECT @p_VL_VALOR_PECA = ISNULL(VL_PECA,0) FROM TB_PECA(NOLOCK) WHERE CD_PECA = @p_CD_PECA
		END 

		IF (@p_QT_PECA IS NULL)
		BEGIN 
			SELECT @p_QT_PECA = 0
		END 

		INSERT INTO dbo.tbEstoqueMovi
		        ( CD_TP_MOVIMENTACAO,
		          ID_OS,
		          DT_MOVIMENTACAO,
		          ID_ESTOQUE,
		          CD_PECA,
		          QT_PECA,
		          ID_USU_MOVI,
		          ID_ESTOQUE_ORIGEM,
		          TP_ENTRADA_SAIDA,
		          CD_CLIENTE,
				  VL_VALOR_PECA,
		          nidUsuarioAtualizacao,
		          dtmDataHoraAtualizacao )
		VALUES
		        ( @p_CD_TP_MOVIMENTACAO,
		          @p_ID_OS,
		          @p_DT_MOVIMENTACAO,
		          @p_ID_ESTOQUE,
		          @p_CD_PECA,
		          @p_QT_PECA,
		          @p_ID_USU_MOVI,
		          @p_ID_ESTOQUE_ORIGEM,
		          @p_TP_ENTRADA_SAIDA,
		          @p_CD_CLIENTE,
				  ISNULL(@p_VL_VALOR_PECA,0),
		          @p_nidUsuarioAtualizacao,
		          GETDATE()
		          )
			

			SET @p_ID_ESTOQUE_MOVI = @@IDENTITY
	
			EXECUTE dbo.prcLogGravar 
						@p_nidLog					= @nidLog,
						@p_nidUsuarioAtualizacao	= @p_nidUsuarioAtualizacao,
						@p_ccdAcao					= 'I',
						@p_cnmTabela				= 'tbEstoqueMovi',
						@p_nidPK					= @p_ID_ESTOQUE_MOVI,
						@p_nidLogReturn				= @nidLog OUTPUT
        
		COMMIT TRANSACTION
	
	END TRY

	BEGIN CATCH

		SELECT	@cdsErrorMessage	= ERROR_MESSAGE(),
				@nidErrorSeverity	= ERROR_SEVERITY(),
				@nidErrorState		= ERROR_STATE();

		ROLLBACK TRANSACTION

		-- Use RAISERROR inside the CATCH block to return error
		-- information about the original error that caused
		-- execution to jump to the CATCH block.
		RAISERROR (@cdsErrorMessage, -- Message text.
				   @nidErrorSeverity, -- Severity.
				   @nidErrorState -- State.
				   )

	END CATCH

END



