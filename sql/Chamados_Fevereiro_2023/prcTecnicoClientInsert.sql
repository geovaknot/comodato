USE [COMODATOHOM]
GO
/****** Object:  StoredProcedure [dbo].[prcTecnicoClienteInsert]    Script Date: 29/03/2023 10:42:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Alex Natalino
-- Create date: 12/03/2018
-- Description:	Inclusão de Dados na Tabela de
--              TB_Tecnico_Cliente
-- =============================================
ALTER PROCEDURE [dbo].[prcTecnicoClienteInsert]	
	@p_CD_CLIENTE				NUMERIC(06),
	@p_CD_TECNICO				VARCHAR(06),
	@p_CD_ORDEM					INT				= NULL,
	@p_nidUsuarioAtualizacao	NUMERIC(18,0)	= NULL
	
AS
BEGIN

	-- Declaração de Variáveis
	DECLARE @cdsErrorMessage	NVARCHAR(4000),
			@nidErrorSeverity	INT,
			@nidErrorState		INT,
			@nidLog				NUMERIC(18,0),

			@ID_AGENDA			BIGINT	= NULL

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY

		BEGIN TRANSACTION

		IF ISNULL(@p_CD_ORDEM, 0) = 0
		BEGIN
			SET @p_CD_ORDEM = (SELECT COUNT(*) + 1 FROM dbo.TB_TECNICO_CLIENTE as TC INNER JOIN TB_TECNICO as T ON TC.CD_TECNICO = T.CD_TECNICO WHERE TC.CD_CLIENTE = @p_CD_Cliente AND T.FL_ATIVO = 'S')
		END      

		INSERT INTO dbo.TB_TECNICO_CLIENTE (
			CD_CLIENTE,
			CD_TECNICO,
			CD_ORDEM )
		VALUES (
			@p_CD_CLIENTE,
			@p_CD_TECNICO,
			@p_CD_ORDEM )

		-- Técnicos na CD_ORDEM = 1 devem gerar registro na tbAgenda
		IF (@p_CD_ORDEM = 1)
		BEGIN
			EXEC dbo.prcAgendaInsert 
			    @p_CD_CLIENTE				= @p_CD_CLIENTE,
			    @p_CD_TECNICO				= @p_CD_TECNICO,
			    @p_NR_ORDENACAO				= NULL,
			    @p_nidUsuarioAtualizacao	= @p_nidUsuarioAtualizacao,
			    @p_ID_AGENDA				= @ID_AGENDA OUTPUT
		END      
	
		--EXECUTE dbo.prcLogGravar 
		--			@p_nidLog					= @nidLog,
		--			@p_nidUsuarioAtualizacao	= @p_nidUsuarioAtualizacao,
		--			@p_ccdAcao					= 'I',
		--			@p_cnmTabela				= 'TB_Tecnico_Cliente',
		--			@p_nidPK					= @p_CD_CLIENTE,
		--			@p_nidLogReturn				= @nidLog OUTPUT
	
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

