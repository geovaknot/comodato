GO
/****** Object:  StoredProcedure [dbo].[prcRptKatPorTecnicoVisita]    Script Date: 11/04/2022 15:13:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ================================================
-- Author:		Paulo Rabelo
-- Alter date: 06/09/2019
-- Description:	Relatório Kat por Técnico Visita
-- ================================================
ALTER PROCEDURE [dbo].[prcRptKatPorTecnicoVisita]
	@p_DT_INICIAL		DATETIME		= NULL,
	@p_DT_FINAL			DATETIME		= NULL,
	@p_CD_TECNICOS		VARCHAR(MAX)	= NULL,
	@p_CD_CLIENTES		VARCHAR(MAX)	= NULL
AS
 
BEGIN

	-- Declaração de Variáveis
	DECLARE @cdsErrorMessage	NVARCHAR(4000),
			@nidErrorSeverity	INT,
			@nidErrorState		INT

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SET FMTONLY OFF;


	BEGIN TRY

		Declare	@DT_HOJE DATETIME = CONVERT(date, DATEADD(HH,-3,GETUTCDATE())) --Hoje Meia Noite

		Declare	@DT_10Dias DATETIME = DATEADD(day, -1, @DT_HOJE) 

		--EXECUTE [dbo].[prcGravaTempoVisitaOS] null , @DT_10Dias --@DT_HOJE

		If Exists(Select * from tempdb..SysObjects Where Name Like '%T_VISITA' ) drop table #T_VISITA;
		--If Exists(Select * from tempdb..SysObjects Where Name like '%T_TEMPOS' ) drop table #T_TEMPOS;
		If Exists(Select * from tempdb..SysObjects Where Name like '%T_QTD_TECNICOS' ) drop table #T_QTD_TECNICOS;
		If Exists(Select * from tempdb..SysObjects Where Name like '%T_TECNICOS_ORDEM' ) drop table #T_TECNICOS_ORDEM;

		
		--SELECT DISTINCT VisitaPadrao.ID_VISITA, VisitaPadrao.CD_TECNICO, VisitaPadrao.CD_Cliente,
		--SUM(DATEDIFF(hour,VisitaPadrao.HR_FIM, VisitaPadrao.HR_INICIO)) AS HorasOS 
		--INTO #T_TEMPOS
		--FROM tbVisitaPadrao VisitaPadrao
		--INNER JOIN tbLogStatusVisitaPadrao lsv 
		--ON lsv.ID_VISITA = VisitaPadrao.ID_VISITA and lsv.ST_STATUS_VISITA IN (3)
		--WHERE VisitaPadrao.ST_STATUS_VISITA in (3) AND lsv.DT_DATA_LOG_OS BETWEEN @p_DT_INICIAL AND @p_DT_FINAL  --AND  EXISTS  (SELECT 1 FROM #T_VISITA v WHERE v.Id_Visita = ID_VISITA)
		--		AND ( VisitaPadrao.CD_TECNICO COLLATE SQL_Latin1_General_CP1_CI_AI	IN (SELECT cdsString FROM dbo.fncGetValuesByString(@p_CD_TECNICOS, ','))		OR @p_CD_TECNICOS		IS NULL )
		--		AND ( VisitaPadrao.CD_CLIENTE IN (SELECT cdsString FROM dbo.fncGetValuesByString(@p_CD_CLIENTES, ','))		OR @p_CD_CLIENTES		IS NULL )
		--GROUP BY VisitaPadrao.ID_VISITA, VisitaPadrao.CD_TECNICO, VisitaPadrao.CD_Cliente


		SELECT vs.ID_VISITA, vs.CD_TECNICO, vs.CD_CLIENTE 
		--, datediff(hour, vs.HR_FIM, vs.HR_INICIO) as Horas 
		, Cast(Convert(float,Replace(vs.HR_FIM,':','.')) - Convert(float,Replace(vs.HR_INICIO,':','.')) as float) as Horas
		INTO #T_VISITA
		FROM tbVisitaPadrao vs
		LEFT JOIN tbLogStatusVisitaPadrao lsv 
		ON lsv.ID_VISITA = vs.ID_VISITA and lsv.ST_STATUS_VISITA IN (4,6)
		WHERE vs.ST_STATUS_VISITA IN (4,6) 
				AND vs.DT_DATA_VISITA BETWEEN @p_DT_INICIAL AND @p_DT_FINAL OR (@p_DT_FINAL IS NULL AND @p_DT_INICIAL IS NULL)
				AND ( vs.CD_TECNICO COLLATE SQL_Latin1_General_CP1_CI_AI	IN (SELECT cdsString FROM dbo.fncGetValuesByString(@p_CD_TECNICOS, ','))		OR @p_CD_TECNICOS		IS NULL )
				AND ( vs.CD_CLIENTE IN (SELECT cdsString FROM dbo.fncGetValuesByString(@p_CD_CLIENTES, ','))		OR @p_CD_CLIENTES		IS NULL )
		GROUP BY vs.ID_VISITA, vs.CD_TECNICO, vs.CD_Cliente, vs.HR_FIM, vs.HR_INICIO
		


		select CD_CLIENTE, count(CD_ORDEM) as QtdTecnicos 
		INTO #T_QTD_TECNICOS
		from TB_TECNICO_CLIENTE
		GROUP BY CD_CLIENTE

		select CD_CLIENTE, CD_TECNICO, CD_ORDEM
		INTO #T_TECNICOS_ORDEM
		from TB_TECNICO_CLIENTE

		SELECT TB_TECNICO.CD_TECNICO 
			, TB_TECNICO.NM_TECNICO
			, TB_TECNICO_CLIENTE.CD_CLIENTE
			, TB_Cliente.NM_CLIENTE
			, ISNULL(SUM(Horas)/3,0) AS KatRealizado

			, CASE WHEN ord.CD_ORDEM <> 1 THEN 0 ELSE TB_CLIENTE.QT_PERIODO END AS KatAno
			, CASE WHEN ord.CD_ORDEM <> 1 THEN 0 ELSE TB_CLIENTE.QT_PERIODO/12.0 END AS KatMes
			--, ISNULL(SUM(HorasOS/3),0) AS KatRealizadoOS
			--, ISNULL(SUM(TempoPortariaHora),0) AS PortariaHoras
			--, ISNULL(SUM(TempoIntegracaoHora),0) AS IntegracaoHoras
			--, ISNULL(SUM(TempoTreinamentoHora),0) AS TreinamentoHoras
			--, ISNULL(SUM(TempoConsultoriaHora),0) AS ConsultoriaHoras
			--, ISNULL(AVG(s.NM_NOTA_PESQ),0) AS PesquisaSatisfacao
			, ISNULL(qtd.QtdTecnicos,0) as QtdTecnicos
			, CASE WHEN ord.CD_ORDEM <> 1 THEN 0 ELSE ISNULL(TB_CLIENTE.QT_PERIODO, 0) END as KatAnoPorTecnico
			, CASE WHEN ord.CD_ORDEM <> 1 THEN 0 ELSE ISNULL((TB_CLIENTE.QT_PERIODO/12.0), 0) END as KatMesPorTecnico
			, CAST(ord.CD_ORDEM AS VARCHAR) OrdemTecnico
		FROM TB_TECNICO_CLIENTE 
			LEFT JOIN #T_VISITA vs 
					ON vs.CD_TECNICO = TB_TECNICO_CLIENTE.CD_TECNICO AND vs.CD_CLIENTE = TB_TECNICO_CLIENTE.CD_CLIENTE
			--LEFT JOIN #T_TEMPOS os 
			--		ON os.CD_TECNICO = TB_TECNICO_CLIENTE.CD_TECNICO AND os.CD_CLIENTE = TB_TECNICO_CLIENTE.CD_CLIENTE
			--LEFT JOIN tbSatisfResposta s on s.ID_VISITA = vs.ID_VISITA
			INNER JOIN TB_Cliente ON TB_TECNICO_CLIENTE.CD_CLIENTE = TB_Cliente.CD_CLIENTE
			INNER JOIN TB_TECNICO ON TB_TECNICO_CLIENTE.CD_TECNICO = TB_TECNICO.CD_TECNICO
			INNER JOIN #T_QTD_TECNICOS qtd on qtd.CD_CLIENTE = TB_TECNICO_CLIENTE.CD_CLIENTE
			INNER JOIN #T_TECNICOS_ORDEM ord on ord.CD_TECNICO = TB_TECNICO.CD_TECNICO 
				AND ord.CD_CLIENTE = TB_CLIENTE.CD_CLIENTE
		WHERE 
			( TB_TECNICO_CLIENTE.CD_TECNICO COLLATE SQL_Latin1_General_CP1_CI_AI	IN (SELECT cdsString FROM dbo.fncGetValuesByString(@p_CD_TECNICOS, ','))		OR @p_CD_TECNICOS		IS NULL )
			AND ( TB_TECNICO_CLIENTE.CD_CLIENTE IN (SELECT cdsString FROM dbo.fncGetValuesByString(@p_CD_CLIENTES, ','))		OR @p_CD_CLIENTES		IS NULL )
			--AND TB_TECNICO_CLIENTE.CD_ORDEM = 1
			AND TB_TECNICO.FL_ATIVO = 'S'
		GROUP BY 
			TB_TECNICO.CD_TECNICO,TB_TECNICO.NM_TECNICO, TB_TECNICO_CLIENTE.CD_CLIENTE ,TB_Cliente.NM_CLIENTE, TB_CLIENTE.QT_PERIODO, QtdTecnicos, ord.CD_ORDEM
		ORDER BY 
			NM_TECNICO,NM_CLIENTE

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


--drop table #T_QTD_TECNICOS

--drop table #T_VISITA

--drop table #T_TECNICOS_ORDEM

--drop table #T_TEMPOS

--select datediff(hour, (select top 1 dtmDataHoraAtualizacao from tbLogStatusVisitaPadrao where ID_VISITA = 10682 AND ST_STATUS_VISITA = 2),(select top 1 dtmDataHoraAtualizacao from tbLogStatusVisitaPadrao where ID_VISITA = 10682 AND ST_STATUS_VISITA = 3)) as Horas
--select * from tbVisitaPadrao order by dtmDataHoraAtualizacao desc
--select * from tbLogStatusVisitaPadrao where ID_VISITA = 10682
