GO
/****** Object:  StoredProcedure [dbo].[prcClienteInsert]    Script Date: 25/04/2023 16:28:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Alex Natalino
-- Create date: 12/03/2018
-- Description:	Inclusão de Dados na Tabela de
--              TB_Cliente
-- =============================================
ALTER PROCEDURE [dbo].[prcClienteInsert]	
	@p_CD_CLIENTE					NUMERIC(6,0)	= NULL,
	@p_CD_GRUPO						VARCHAR(10)		= NULL,
	@p_CD_RAC						VARCHAR(05)		= NULL,
	@p_CD_VENDEDOR					NUMERIC(6,0)	= NULL,
	@p_NR_CNPJ						VARCHAR(20)		= NULL,
	@p_NM_CLIENTE					VARCHAR(50)		= NULL,
	@p_EN_ENDERECO					VARCHAR(50)		= NULL,
	@p_EN_BAIRRO					VARCHAR(50)		= NULL,
	@p_EN_CIDADE					VARCHAR(50)		= NULL,
	@p_EN_ESTADO					VARCHAR(03)		= NULL,
	@p_EN_CEP						VARCHAR(10)		= NULL,
	@p_CD_REGIAO					VARCHAR(06)		= NULL,
	@p_CD_FILIAL					VARCHAR(05)		= NULL,
	@p_CD_ABC						VARCHAR(02)		= NULL,
	@p_CL_CLIENTE					VARCHAR(05)		= NULL,
	@p_TX_TELEFONE					VARCHAR(25)		= NULL,
	@p_TX_FAX						VARCHAR(25)		= NULL,
	@p_TX_EMAIL						VARCHAR(100)	= NULL,
	@p_DT_DESATIVACAO				DATETIME		= NULL,
	@p_CD_EXECUTIVO					NUMERIC(3,0)	= NULL,
	@p_BPCS							BINARY(1)		= NULL,
	@p_QT_PERIODO					INT				= NULL,
	@p_FL_PESQ_SATISF				nchar(2)		= NULL,
	@p_ID_SEGMENTO					bigint			= NULL,
	@p_nidUsuario					NUMERIC(18,0)	= NULL,
	@p_FL_KAT_FIXO					BIT				= NULL,
	@p_DS_CLASSIFICACAO_KAT			VARCHAR(02)		= NULL,
	@p_nidUsuarioAtualizacao		NUMERIC(18,0)	= NULL,
	@p_ID_USUARIO_TECNICOREGIONAL	bigint			= NULL,
	@p_CD_BCPS						varchar(8)		= NULL,
	@p_FL_AtivaPlanoZero			varchar(1)		= NULL,
	@p_QTD_PeriodoPlanoZero			numeric(2)		= NULL
	
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

		IF (@p_BPCS = 1)
		begin
			SET @p_CD_VENDEDOR = (SELECT CD_VENDEDOR FROM TB_VENDEDOR WHERE ID_USUARIO = @p_CD_VENDEDOR)

			IF (@p_ID_SEGMENTO = 0 OR @p_ID_SEGMENTO IS NULL)
			begin
				SET @p_ID_SEGMENTO = (SELECT ID_SEGMENTO FROM tbSegmento WHERE ds_segmentomin = 'OUTRO')
			end
		end

		INSERT INTO TB_Cliente
			(
			CD_CLIENTE,
			CD_GRUPO,
			CD_RAC,
			CD_VENDEDOR,
			NR_CNPJ,
			NM_CLIENTE,
			EN_ENDERECO,
			EN_BAIRRO,
			EN_CIDADE,
			EN_ESTADO,
			EN_CEP,
			CD_REGIAO,
			CD_FILIAL,
			CD_ABC,
			CL_CLIENTE,
			TX_TELEFONE,
			TX_FAX,
			TX_EMAIL,
			DT_DESATIVACAO,
			CD_EXECUTIVO,
			BPCS,
			QT_PERIODO,
			FL_PESQ_SATISF,
			ID_SEGMENTO,
			nidUsuario,
			FL_KAT_FIXO,
			DS_CLASSIFICACAO_KAT,
			ID_USUARIO_TECNICOREGIONAL,
			CD_BCPS,
			FL_AtivaPlanoZero,
			QTD_PeriodoPlanoZero
			)
		VALUES
			(			
			@p_CD_CLIENTE,
			@p_CD_GRUPO,
			@p_CD_RAC,
			@p_CD_VENDEDOR,
			@p_NR_CNPJ,
			@p_NM_CLIENTE,
			@p_EN_ENDERECO,
			@p_EN_BAIRRO,
			@p_EN_CIDADE,
			@p_EN_ESTADO,
			@p_EN_CEP,
			@p_CD_REGIAO,
			@p_CD_FILIAL,
			@p_CD_ABC,
			@p_CL_CLIENTE,
			@p_TX_TELEFONE,
			@p_TX_FAX,
			@p_TX_EMAIL,
			@p_DT_DESATIVACAO,
			@p_CD_EXECUTIVO,
			@p_BPCS,
			@p_QT_PERIODO,
			@p_FL_PESQ_SATISF,
			@p_ID_SEGMENTO,
			@p_nidUsuario,
			@p_FL_KAT_FIXO,
			@p_DS_CLASSIFICACAO_KAT,
			@p_ID_USUARIO_TECNICOREGIONAL,
			@p_CD_BCPS,
			@p_FL_AtivaPlanoZero,
			@p_QTD_PeriodoPlanoZero
			)

		--SET @p_CD_Cliente = @@IDENTITY
	
		EXECUTE dbo.prcLogGravar 
					@p_nidLog					= @nidLog,
					@p_nidUsuarioAtualizacao	= @p_nidUsuarioAtualizacao,
					@p_ccdAcao					= 'I',
					@p_cnmTabela				= 'TB_Cliente',
					@p_nidPK					= @p_CD_Cliente,
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






