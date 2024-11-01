GO
/****** Object:  StoredProcedure [dbo].[prcTecnicoInsert]    Script Date: 10/06/2022 10:28:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Alex Natalino
-- Create date: 12/03/2018
-- Description:	Inclusão de Dados na Tabela de
--              TB_Tecnico
-- Author:		Michelle Fonseca
-- Create date: 16/01/2019
-- Description:	Inclusao do Usuario Tec.Regional
--              TB_Tecnico
-- =============================================
ALTER PROCEDURE [dbo].[prcTecnicoInsert]	
	@p_CD_Tecnico				VARCHAR(06)		= NULL,	
	@p_NM_Tecnico				VARCHAR(50)		= NULL,
	@p_EN_Endereco				VARCHAR(100)	= NULL, 
	@p_EN_Bairro				VARCHAR(30)		= NULL,
	@p_EN_Cidade				VARCHAR(30)		= NULL,
	@p_EN_Estado				VARCHAR(02)		= NULL,
	@p_EN_CEP					VARCHAR(09)		= NULL,
	@p_TX_Telefone				VARCHAR(20)		= NULL,
	@p_TX_FAX					VARCHAR(20)		= NULL,
	@P_TX_Email					VARCHAR(255)	= NULL,
	@p_TP_Tecnico				VARCHAR(01)		= NULL,
	@p_VL_Custo_Hora			NUMERIC(14,2)	= NULL,
	@p_FL_Ativo					VARCHAR(01)		= NULL,
	@p_ID_USUARIO_COORDENADOR	NUMERIC(18,0)	= NULL,
	@p_ID_USUARIO				NUMERIC(18,0)	= NULL,
	@p_CD_EMPRESA				NUMERIC(18,0)	= NULL,
	@p_FL_Ferias				VARCHAR(01)		= NULL,
	@p_nidUsuarioAtualizacao	NUMERIC(18,0)	= NULL,
	@p_ID_USUARIO_SUPERVISOR	NUMERIC(18,0)	= NULL,
	@p_NM_REDUZIDO				VARCHAR(20)		= NULL,
	@p_CD_BCPS					VARCHAR(8)		= NULL
	
AS
BEGIN

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

		INSERT INTO TB_Tecnico
			(
			CD_Tecnico,
			NM_Tecnico,
			EN_Endereco,
			EN_Bairro,
			EN_Cidade,
			EN_Estado,
			EN_CEP,
			TX_Telefone,
			TX_FAX,
			TX_Email,
			TP_Tecnico,
			VL_Custo_Hora,
			FL_Ativo,
			ID_USUARIO_COORDENADOR,
			ID_USUARIO_TECNICOREGIONAL,
			ID_USUARIO,
			CD_EMPRESA,
			FL_FERIAS,
			NM_REDUZIDO,
			CD_BCPS
			)
		VALUES
			(			
			@p_CD_Tecnico,
			@p_NM_Tecnico,
			@p_EN_Endereco, 
			@p_EN_Bairro,
			@p_EN_Cidade,
			@p_EN_Estado,
			@p_EN_CEP,
			@p_TX_Telefone,
			@p_TX_FAX,
			@P_TX_Email,
			@p_TP_Tecnico,
			@p_VL_Custo_Hora,
			@p_FL_Ativo,
			@p_ID_USUARIO_COORDENADOR,
			@p_ID_USUARIO_SUPERVISOR,
			@p_ID_USUARIO,
			@p_CD_EMPRESA,
			@p_FL_Ferias,
			@p_NM_REDUZIDO, 
			@p_CD_BCPS
			)

		--SET @p_CD_Tecnico = @@IDENTITY
	
		EXECUTE dbo.prcLogGravar 
					@p_nidLog					= @nidLog,
					@p_nidUsuarioAtualizacao	= @p_nidUsuarioAtualizacao,
					@p_ccdAcao					= 'I',
					@p_cnmTabela				= 'TB_Tecnico',
					@p_nidPK					= @p_CD_Tecnico,
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

