GO
/****** Object:  StoredProcedure [dbo].[prcTecnicoUpdate]    Script Date: 10/06/2022 10:42:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Alex Natalino
-- Create date: 12/03/2018
-- Description:	CREATEação de Dados na Tabela de
--              TB_Tecnico
-- =============================================
ALTER PROCEDURE [dbo].[prcTecnicoUpdate]
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
	@p_ID_USUARIO_SUPERVISOR	NUMERIC(18,0)	= NULL,
	@p_ID_USUARIO				NUMERIC(18,0)	= NULL,
	@p_CD_EMPRESA				NUMERIC(18,0)	= NULL,
	@p_FL_Ferias				VARCHAR(01)		= NULL,
	@p_nidUsuarioAtualizacao	NUMERIC(18,0)	= NULL,
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
		
		--EXECUTE dbo.prcLogGravar 
		--		@p_nidLog					= @nidLog,
		--		@p_nidUsuarioAtualizacao	= @p_nidUsuarioAtualizacao,
		--		@p_ccdAcao					= 'U',
		--		@p_cnmTabela				= 'TB_Tecnico',
		--		@p_nidPK					= @p_CD_Tecnico,
		--		@p_nidLogReturn				= @nidLog OUTPUT
				
		UPDATE	TB_Tecnico 
		SET		CD_Tecnico				= ISNULL(@p_CD_Tecnico,		CD_Tecnico),
				NM_Tecnico				= ISNULL(@p_NM_Tecnico,		NM_Tecnico),
				EN_Endereco				= ISNULL(@p_EN_Endereco,	EN_Endereco), 
				EN_Bairro				= ISNULL(@p_EN_Bairro,		EN_Bairro),
				EN_Cidade				= ISNULL(@p_EN_Cidade,		EN_Cidade),
				EN_Estado				= ISNULL(@p_EN_Estado,		EN_Estado),
				EN_CEP					= ISNULL(@p_EN_CEP,			EN_CEP),
				TX_Telefone				= ISNULL(@p_TX_Telefone,	TX_Telefone),
				TX_FAX					= ISNULL(@p_TX_FAX,			TX_FAX),
				TX_Email				= ISNULL(@P_TX_Email,		TX_Email),
				TP_Tecnico				= ISNULL(@p_TP_Tecnico,		TP_Tecnico),
				VL_Custo_Hora			= ISNULL(@p_VL_Custo_Hora,	VL_Custo_Hora),
				FL_Ativo				= ISNULL(@p_FL_Ativo,		FL_Ativo),
				ID_USUARIO_COORDENADOR	= @p_ID_USUARIO_COORDENADOR,
				ID_USUARIO_TECNICOREGIONAL =   @p_ID_USUARIO_SUPERVISOR,
				ID_USUARIO				= @p_ID_USUARIO,
				CD_EMPRESA				= @p_CD_EMPRESA,
				FL_FERIAS				= @p_FL_Ferias,
				NM_REDUZIDO				= ISNULL(@p_NM_REDUZIDO,	NM_REDUZIDO),
				CD_BCPS					= @p_CD_BCPS
			
		WHERE	CD_Tecnico				= @p_CD_Tecnico
		         	
		EXECUTE dbo.prcLogGravar 
				@p_nidLog					= @nidLog,
				@p_nidUsuarioAtualizacao	= @p_nidUsuarioAtualizacao,
				@p_ccdAcao					= 'U',
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

