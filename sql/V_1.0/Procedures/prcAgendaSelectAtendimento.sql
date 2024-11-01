GO
/****** Object:  StoredProcedure [dbo].[prcAgendaSelectAtendimento]    Script Date: 08/11/2021 08:35:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Alex Natalino
-- Create date: 12/03/2018
-- Description:	Seleção de dados na tabela 
--              TB_Tecnico_Cliente
-- ============================================= 
ALTER PROCEDURE [dbo].[prcAgendaSelectAtendimento]
	@p_nidUsuario					NUMERIC(18,0)	= NULL,
	@p_CD_CLIENTE					INT				= NULL,	
	@p_CD_TECNICO					VARCHAR(06) = NULL,
	@p_nvlQtdeTecnicos				INT				= NULL,
	@p_ST_TP_STATUS_VISITA_OS		INT				= NULL,
	@p_CD_REGIAO					VARCHAR(02)		= NULL,
	@pDtInicial						DAtetime		= null,
	@pDtFinal						DAtetime		= null
	--@p_ID_OS						BIGINT			= NULL   
AS
BEGIN

	-- Declaração de Variáveis
	DECLARE @cdsErrorMessage	NVARCHAR(4000),
			@nidErrorSeverity	INT,
			@nidErrorState		INT,
			@ID_AGENDA			BIGINT,
			@CD_CLIENTE			NUMERIC(06)	= NULL,
			@CD_ORDEM			INT			= NULL,
			@vigenciaInicial	DATETIME,
			@vigenciaFinal		DATETIME

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY

		SELECT @vigenciaInicial = CONVERT(DATETIME, cvlParametro) FROM dbo.tbParametro WHERE ccdParametro = 'vigenciaINICIAL'
		SELECT @vigenciaFinal = CONVERT(DATETIME, cvlParametro) FROM dbo.tbParametro WHERE ccdParametro = 'vigenciaFINAL'

		IF (@vigenciaInicial IS NULL)
			SELECT @vigenciaInicial = CONVERT(DATETIME, CONVERT(VARCHAR, YEAR(DATEADD(HH,-3,GETUTCDATE()))) + '-01-01')

		IF (@vigenciaFinal IS NULL)
			SELECT @vigenciaFinal = CONVERT(DATETIME, CONVERT(VARCHAR, YEAR(DATEADD(HH,-3,GETUTCDATE()))) + '-12-31') 
		
		CREATE TABLE #tbCliente
			(
				CD_CLIENTE				NUMERIC(6,0)	NULL,
				NM_CLIENTE				VARCHAR(50)		NULL,
				EN_CIDADE				VARCHAR(50)		NULL,
				EN_ESTADO				VARCHAR(03)		NULL,
				CD_REGIAO				VARCHAR(02)		NULL,
				DS_REGIAO				VARCHAR(30)		NULL,
				DT_DESATIVACAO			DATETIME		NULL,
				nvlQtdeTecnicos			INT				NULL,
				CD_TECNICO_PRINCIPAL	VARCHAR(06)		NULL,
				NM_TECNICO_PRINCIPAL	VARCHAR(50)		NULL,
				QT_PERIODO				INT				NULL,
				CD_ORDEM				INT				NULL,
				FL_CRIAR_REG_AGENDA		CHAR(1)			NULL
			)

		--INSERT INTO #tbCliente
		--EXEC dbo.prcTecnicoClienteSelectQtdeTecnicos 
		--	@p_nidUsuario = @p_nidUsuario,
		--	@p_CD_Cliente = @p_CD_CLIENTE,
		--	@p_CD_Tecnico = @p_CD_TECNICO,
		--	@p_nvlQtdeTecnicos = @p_nvlQtdeTecnicos

		INSERT INTO #tbCliente
		(
			CD_CLIENTE,
			NM_CLIENTE,
			EN_CIDADE,
			EN_ESTADO,
			CD_REGIAO,
			DS_REGIAO,
			DT_DESATIVACAO,
			QT_PERIODO,
			CD_TECNICO_PRINCIPAL,
			NM_TECNICO_PRINCIPAL,
			CD_ORDEM,
			FL_CRIAR_REG_AGENDA
		)
		SELECT 
			CD_CLIENTE,
			NM_CLIENTE,
			EN_CIDADE,
			EN_ESTADO,
			CD_REGIAO,
			DS_REGIAO,
			DT_DESATIVACAO,
			QT_PERIODO,
			(SELECT TOP 1 T.CD_TECNICO 
				FROM dbo.TB_TECNICO AS T
				INNER JOIN dbo.TB_TECNICO_CLIENTE AS TC
					ON T.CD_TECNICO = TC.CD_TECNICO
				WHERE TC.CD_CLIENTE = FNC.CD_CLIENTE
				AND TC.CD_ORDEM = 1 
			),
			(SELECT TOP 1 T.NM_TECNICO 
				FROM dbo.TB_TECNICO AS T
				INNER JOIN dbo.TB_TECNICO_CLIENTE AS TC
					ON T.CD_TECNICO = TC.CD_TECNICO
				WHERE TC.CD_CLIENTE = FNC.CD_CLIENTE
				AND TC.CD_ORDEM = 1 
			),
			CD_ORDEM,
			FL_CRIAR_REG_AGENDA
		FROM dbo.fncRetornaTecnicoClienteAgenda(@p_CD_TECNICO) AS FNC 
		ORDER BY CD_ORDEM

		-- Insere na tbAgenda os registros retornados com FL_CRIAR_REG_AGENDA = 'S' indicando que são registros de agendas de outros usuários principais e precisam ser incluídos na agenda do usuário atual
		-- para permitir que o mesmo possa alterar a ordem na tela de Agenda Atendimento
		DECLARE C1 CURSOR LOCAL FORWARD_ONLY FAST_FORWARD FOR
		SELECT	CD_CLIENTE,
				CD_ORDEM
		FROM #tbCliente
		WHERE FL_CRIAR_REG_AGENDA = 'S'
		ORDER BY CD_ORDEM

		OPEN C1
		FETCH NEXT FROM C1
			INTO @CD_CLIENTE, @CD_ORDEM
		WHILE @@FETCH_STATUS = 0
		BEGIN
			EXEC dbo.prcAgendaInsert 
			    @p_CD_Cliente = @CD_CLIENTE,
			    @p_CD_Tecnico = @p_CD_TECNICO,
			    @p_NR_ORDENACAO = @CD_ORDEM,
			    @p_nidUsuarioAtualizacao = @p_nidUsuario,
			    @p_ID_AGENDA = @ID_AGENDA OUTPUT 
			
			FETCH NEXT FROM C1
				INTO @CD_CLIENTE, @CD_ORDEM;
		END;
		CLOSE C1;
		DEALLOCATE C1;

		--SELECT 
		--	dbo.tbVisita.ID_VISITA, 
		--	dbo.tbVisita.ST_TP_STATUS_VISITA_OS,
		--	--dbo.tbOS.ID_OS,
		--	dbo.tbTpStatusVisitaOS.DS_TP_STATUS_VISITA_OS,
		--	dbo.tbAgenda.ID_AGENDA,
		--	dbo.tbAgenda.CD_CLIENTE,
		--	#tbCliente.NM_CLIENTE,
		--	#tbCliente.EN_CIDADE,
		--	#tbCliente.EN_ESTADO,
		--	#tbCliente.CD_REGIAO,
		--	#tbCliente.DS_REGIAO,
		--	#tbCliente.DT_DESATIVACAO,
		--	dbo.tbVisita.DT_DATA_VISITA,
		--	#tbCliente.CD_TECNICO_PRINCIPAL,
		--	#tbCliente.NM_TECNICO_PRINCIPAL,
		--	#tbCliente.QT_PERIODO,
		--	--dbo.tbAgenda.NR_ORDENACAO,
		--	#tbCliente.CD_ORDEM
		--FROM dbo.tbAgenda 
		--LEFT OUTER JOIN dbo.tbVisita 
		--ON dbo.tbAgenda.CD_CLIENTE = dbo.tbVisita.CD_CLIENTE
		--AND dbo.tbVisita.ID_VISITA = (SELECT MAX(V.ID_VISITA) FROM dbo.tbVisita V WHERE V.CD_CLIENTE = dbo.tbAgenda.CD_CLIENTE)
		--LEFT OUTER JOIN dbo.tbTpStatusVisitaOS
		--ON dbo.tbVisita.ST_TP_STATUS_VISITA_OS = dbo.tbTpStatusVisitaOS.ST_TP_STATUS_VISITA_OS
		--AND dbo.tbTpStatusVisitaOS.FL_STATUS_OS = 'N'
		--INNER JOIN #tbCliente
		--ON dbo.tbAgenda.CD_CLIENTE = #tbCliente.CD_CLIENTE
		--WHERE	( dbo.tbAgenda.CD_CLIENTE				= @p_CD_CLIENTE					OR @p_CD_CLIENTE				IS NULL )
		--AND		( dbo.tbVisita.ST_TP_STATUS_VISITA_OS	= @p_ST_TP_STATUS_VISITA_OS		OR @p_ST_TP_STATUS_VISITA_OS	IS NULL )
		--AND		( #tbCliente.CD_REGIAO					= @p_CD_REGIAO					OR @p_CD_REGIAO					IS NULL )
		---- Somente Clientes ATIVOS ou também os INATIVOS com Status de Visita ABERTA
		--AND ( #tbCliente.DT_DESATIVACAO IS NULL OR (NOT #tbCliente.DT_DESATIVACAO IS NULL AND dbo.tbVisita.ST_TP_STATUS_VISITA_OS = 2) )
		--ORDER BY 
		--	dbo.tbAgenda.NR_ORDENACAO

		SELECT 
			DISTINCT

			--fnc.*,
			fnc.CD_CLIENTE,
			fnc.NM_CLIENTE,
			fnc.EN_CIDADE,
			fnc.EN_ESTADO,
			fnc.CD_REGIAO,
			fnc.DS_REGIAO,
			fnc.DT_DESATIVACAO,
			fnc.CD_TECNICO_PRINCIPAL,
			fnc.NM_TECNICO_PRINCIPAL,
			fnc.QT_PERIODO,
			fnc.CD_ORDEM AS NR_ORDENACAO,
			tbOsPadraoResult.ID_OS, 
			tbOsPadraoResult.ST_STATUS_OS,
			tbOsPadraoResult.DT_DATA_OS,
			tbOsPadraoResultStatus.DS_STATUS_OS,
			dbo.tbAgenda.ID_AGENDA
		FROM #tbCliente AS fnc 
		inner JOIN dbo.tbOsPadrao tbOsPadraoResult
			ON fnc.CD_CLIENTE = tbOsPadraoResult.CD_CLIENTE
			AND (tbOsPadraoResult.DT_DATA_OS >= @pDtInicial or @pDtInicial is null)
			AND (tbOsPadraoResult.DT_DATA_OS <= @pDtFinal or @pDtFinal is null)
			--AND tbOsPadraoResult.ID_OS = (SELECT MAX(V.ID_OS) FROM dbo.tbOsPadrao V WHERE V.CD_CLIENTE = tbOsPadraoResult.CD_CLIENTE) --AND V.CD_TECNICO = tbOsPadraoResult.CD_TECNICO)
			--AND		tbOsPadraoResult.DT_DATA_OS BETWEEN @vigenciaInicial AND @vigenciaFinal -- @far-29-10-18-Aplica o filtro quando há visita 

		inner JOIN dbo.tbTpStatusOSPadrao tbOsPadraoResultStatus
			ON tbOsPadraoResult.ST_STATUS_OS = tbOsPadraoResultStatus.ST_STATUS_OS
		INNER JOIN dbo.tbAgenda
			ON dbo.tbAgenda.CD_CLIENTE = fnc.CD_CLIENTE
			AND dbo.tbAgenda.CD_TECNICO = @p_CD_TECNICO

		INNER JOIN TB_ATIVO_CLIENTE ON fnc.CD_CLIENTE = TB_ATIVO_CLIENTE.CD_CLIENTE

		WHERE	--( dbo.tbAgenda.CD_CLIENTE				= @p_CD_CLIENTE					OR @p_CD_CLIENTE				IS NULL )
				( tbOsPadraoResult.ST_STATUS_OS			= @p_ST_TP_STATUS_VISITA_OS		OR @p_ST_TP_STATUS_VISITA_OS	IS NULL )
		AND		( fnc.CD_REGIAO							= @p_CD_REGIAO					OR @p_CD_REGIAO					IS NULL )
		-- Somente Clientes ATIVOS ou também os INATIVOS com Status de Visita ABERTA
		AND		( fnc.DT_DESATIVACAO					IS NULL							OR (NOT fnc.DT_DESATIVACAO IS NULL AND tbOsPadraoResult.ST_STATUS_OS = 2) )
		--AND		tbVisita.DT_DATA_VISITA BETWEEN @vigenciaInicial AND @vigenciaFinal -- @far-29-10-18-Não exibe clientes onde é tecnico principal

		AND ( dbo.TB_ATIVO_CLIENTE.DT_DEVOLUCAO IS NULL)
		ORDER BY fnc.CD_ORDEM

		If(OBJECT_ID('tempdb..#tbCliente') Is Not Null)
		BEGIN
			DROP TABLE #tbCliente
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



--select * from tbTpstatusospadrao
--select #tbCliente.*, tbosPadTeste.* from #tbCliente
--	inner join tbOsPadrao tbosPadTeste 
--	on tbosPadTeste.CD_CLIENTE = #tbCliente.CD_CLIENTE
--select * from tbOsPadrao where cd_tecnico = '347823'
