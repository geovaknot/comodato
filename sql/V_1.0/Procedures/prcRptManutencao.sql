GO
/****** Object:  StoredProcedure [dbo].[prcRptManutencao]    Script Date: 03/12/2021 14:14:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




-- ================================================
-- Author:		Paulo Rabelo
-- Alter date:	22/04/2020 
-- Description:	Relatório "Manutenção" 
-- ================================================


ALTER PROCEDURE [dbo].[prcRptManutencao]
	 @pDtInicial	DATE = NULL
	,@pDtFinal		DATE = NULL
	,@pCD_GRUPO		VARCHAR(10) = NULL
	,@pCD_CLIENTE	NUMERIC(6,0) = NULL
	,@pCD_TECNICO	VARCHAR(6) = NULL
	,@CD_ATIVO_FIXO	VARCHAR(6) = NULL
	,@pID_VISITA	BIGINT = NULL
	,@pID_OS		BIGINT = NULL
	
AS
BEGIN

	SET NOCOUNT ON;
	SET FMTONLY OFF;


	-- Calcula os tempos de ontem e Hoje porque o Job calcula só até meia noite
	Declare	@DT_HOJE DATETIME = CONVERT(date, DATEADD(HH,-3,GETUTCDATE())) --Hoje Meia Noite

	Declare	@DT_10Dias DATETIME = DATEADD(day, -1, @DT_HOJE) 

	--EXECUTE [dbo].[prcGravaTempoVisitaOS] null , @DT_10Dias --@DT_HOJE

	-- Tabela temporária: Contagem de Peças por OS 
	If Exists(Select * from tempdb..SysObjects Where Name Like '%T_RelManut_ContaPecaOS' ) drop table #T_RelManut_ContaPecaOS;

	SELECT 
		REL.ID_OS, IIF(ISNULL(COUNT(RPE.ID_PECA_OS),1) = 0,1,ISNULL(COUNT(RPE.ID_PECA_OS),1) ) AS ContaPeca 
	INTO #T_RelManut_ContaPecaOS
	FROM     tbOSPadrao REL 
		INNER JOIN     TB_TECNICO TEC ON TEC.CD_TECNICO = REL.CD_TECNICO 
		INNER JOIN     TB_CLIENTE CLI ON REL.CD_CLIENTE = CLI.CD_CLIENTE
		LEFT JOIN     tbUsuario usu ON usu.nidUsuario = CLI.ID_USUARIO_TECNICOREGIONAL
		LEFT JOIN     TB_GRUPO GRP ON CLI.CD_GRUPO = GRP.CD_GRUPO  
		--LEFT JOIN     tbOS REQ ON REQ.ID_VISITA = REL.ID_OS
		LEFT JOIN     tbPecaOS RPE ON RPE.ID_OS = REL.ID_OS 
		LEFT JOIN     TB_ATIVO_FIXO EQP ON REL.CD_ATIVO_FIXO = EQP.CD_ATIVO_FIXO 
		LEFT JOIN     TB_MODELO MODE ON MODE.CD_MODELO = EQP.CD_MODELO 
		LEFT JOIN     TB_PECA PEC ON PEC.CD_PECA = RPE.CD_PECA       
	WHERE
		TEC.FL_ATIVO = 'S' 
		AND (@pCD_GRUPO IS NULL OR CLI.CD_GRUPO = @pCD_GRUPO)
		AND (@pCD_CLIENTE IS NULL OR CLI.CD_CLIENTE = @pCD_CLIENTE)
		AND (@pCD_TECNICO IS NULL OR TEC.CD_TECNICO = @pCD_TECNICO)
		AND (@CD_ATIVO_FIXO IS NULL OR REL.CD_ATIVO_FIXO = @CD_ATIVO_FIXO)	
		AND (
				(@pDtInicial IS NULL	OR CONVERT(DATE,REL.DT_DATA_OS) >= @pDtInicial) AND 
				(@pDtFinal IS NULL		OR CONVERT(DATE,REL.DT_DATA_OS) <= @pDtFinal)
			)
		AND (@pID_VISITA IS NULL OR REL.ID_OS=@pID_OS)
		--AND (@pID_OS IS NULL OR REQ.ID_OS = @pID_OS)
		AND ( REL.ST_STATUS_OS IN (3,5) OR REL.ST_STATUS_OS IS NULL)
		--AND ( REQ.ST_TP_STATUS_VISITA_OS = 4 OR REQ.ST_TP_STATUS_VISITA_OS IS NULL)		
	GROUP BY
		REL.ID_OS


	--Query Principal
	SELECT 
		 REL.ID_OS	NR_DOCUMENTO
		--,REL.DT_DATA_VISITA	DT_CRIACAO    
		,TEC.CD_TECNICO     
		,TEC.NM_TECNICO     
		,CLI.CD_CLIENTE     
		,CLI.NM_CLIENTE     
		,REL.DS_RESPONSAVEL as DS_NOME_RESPONSAVEL
		,REL.DS_OBSERVACAO TX_OBS
		,REL.CD_ATIVO_FIXO AS CD_ATIVO_FIXO   
		,MODE.DS_MODELO  
		--, Cast(Convert(float,Replace(REL.HR_FIM,':','.')) - Convert(float,Replace(REL.HR_INICIO,':','.')) as float) HR_TRABALHADAS
		,Cast((Convert(float,Substring(REL.HR_FIM,0,3)) + Convert(float,Substring(REL.HR_FIM,4,2))/60)
		-(Convert(float,Substring(REL.HR_INICIO,0,3)) + Convert(float,Substring(REL.HR_INICIO,4,2))/60) 
			as float) HR_TRABALHADAS
			--,Cast((Convert(float,Substring(REL.HR_FIM,0,3)) + Convert(float,Substring(REL.HR_FIM,4,2))/60) as float) HR_FIM
			--,CAST((Convert(float,Substring(REL.HR_INICIO,0,3)) + Convert(float,Substring(REL.HR_INICIO,4,2))/60) as float) HR_INICIO
		--, Convert(Time,(select top 1 logOS.dt_data_log_os from tbLogStatusOSPadrao logOS where logOS.ST_STATUS_OS = 3 AND logOS.ID_OS = REL.ID_OS)
		--				-(select top 1 logOS.dt_data_log_os from tbLogStatusOSPadrao logOS where logOS.ST_STATUS_OS = 2 AND logOS.ID_OS = REL.ID_OS)) HR_TRABALHADAS
		, dbo.fConverteHora(Cast((Convert(float,Substring(REL.HR_FIM,0,3)) + Convert(float,Substring(REL.HR_FIM,4,2))/60) 
		-(Convert(float,Substring(REL.HR_INICIO,0,3)) + Convert(float,Substring(REL.HR_INICIO,4,2))/60) 
			as float)) AS HR_TRAB_CONV 
		,(TEC.VL_CUSTO_HORA * (Cast((Convert(float,Substring(REL.HR_FIM,0,3)) + Convert(float,Substring(REL.HR_FIM,4,2))/60) 
		-(Convert(float,Substring(REL.HR_INICIO,0,3)) + Convert(float,Substring(REL.HR_INICIO,4,2))/60) 
			as float)) / ContaPecaOS.ContaPeca) VL_MANUTENCAO
		,CASE	REL.CD_TIPO_OS 
				WHEN '1' THEN 'Preventiva'
				WHEN '2' THEN 'Corretiva'
				WHEN '3' THEN 'Instalação'
				WHEN '4' THEN 'Outros'
				--ELSE REL.CD_TIPO_OS 
		 END AS TP_MANUTENCAO
		,PEC.CD_PECA
		,PEC.DS_PECA
        ,RPE.QT_PECA
		,RPE.VL_VALOR_PECA
		,ISNULL(RPE.QT_PECA,0) * ISNULL(RPE.VL_VALOR_PECA,0) VL_TOT_PECA
		,TEC.VL_CUSTO_HORA
		,REL.DT_DATA_OS DT_CRIACAO
		--,lsv.DT_DATA_LOG_OS   DT_CRIACAO
		,usu.cnmNome as TecnicoRegional
		,CASE rpe.CD_TP_ESTOQUE_CLI_TEC 
				WHEN 'T' THEN 'I'
				WHEN 'C' THEN 'C'
		END AS ESTOQUE_PEÇA
	FROM     tbOSPadrao REL 
		INNER JOIN     TB_TECNICO TEC ON TEC.CD_TECNICO = REL.CD_TECNICO 
		INNER JOIN     TB_CLIENTE CLI ON REL.CD_CLIENTE = CLI.CD_CLIENTE
		LEFT JOIN     tbUsuario usu ON usu.nidUsuario = CLI.ID_USUARIO_TECNICOREGIONAL
		LEFT JOIN     TB_GRUPO GRP ON CLI.CD_GRUPO = GRP.CD_GRUPO  
		--LEFT JOIN     tbOS REQ ON REQ.ID_VISITA = REL.ID_OS
		LEFT JOIN     tbPecaOS RPE ON RPE.ID_OS = REL.ID_OS 
		LEFT JOIN     TB_ATIVO_FIXO EQP ON REL.CD_ATIVO_FIXO = EQP.CD_ATIVO_FIXO 
		LEFT JOIN     TB_MODELO MODE ON MODE.CD_MODELO = EQP.CD_MODELO 
		LEFT JOIN     TB_PECA PEC ON PEC.CD_PECA = RPE.CD_PECA       
		LEFT JOIN	#T_RelManut_ContaPecaOS ContaPecaOS ON ContaPecaOS.ContaPeca = REL.ID_OS
		LEFT JOIN dbo.tbLogStatusOSPadrao lsv ON lsv.ID_OS = REL.ID_OS --and lsv.ST_STATUS_OS = 2 
	WHERE
		TEC.FL_ATIVO = 'S' 
		AND (@pCD_GRUPO IS NULL OR CLI.CD_GRUPO = @pCD_GRUPO)
		AND (@pCD_CLIENTE IS NULL OR CLI.CD_CLIENTE = @pCD_CLIENTE)
		AND (@pCD_TECNICO IS NULL OR TEC.CD_TECNICO = @pCD_TECNICO)
		AND (@CD_ATIVO_FIXO IS NULL OR REL.CD_ATIVO_FIXO = @CD_ATIVO_FIXO)	
				AND (
				(@pDtInicial IS NULL	OR CONVERT(DATE,REL.DT_DATA_OS) >= @pDtInicial) AND 
				(@pDtFinal IS NULL		OR CONVERT(DATE,REL.DT_DATA_OS) <= @pDtFinal)
			)
		--AND (@pID_VISITA IS NULL OR REL.ID_OS=@pID_VISITA)
		--AND (@pID_OS IS NULL OR REQ.ID_OS = @pID_OS)
		AND ( REL.ST_STATUS_OS IN (3,5) OR REL.ST_STATUS_OS IS NULL)
		--AND ( lsv.ST_TP_STATUS_VISITA_OS IN (3,4) OR lsv.ST_TP_STATUS_VISITA_OS IS NULL)
		--AND ( REL.ST_STATUS_OS in (3,5) OR REQ.ST_TP_STATUS_VISITA_OS IS NULL)		
	GROUP BY REL.ID_OS
			--,lsv.DT_DATA_LOG_OS
			,REL.DT_DATA_OS
 			--,REL.DT_DATA_VISITA    
 			,TEC.CD_TECNICO     
 			,TEC.NM_TECNICO     
 			,CLI.CD_CLIENTE     
 			,CLI.NM_CLIENTE     
 			,REL.DS_RESPONSAVEL
 			,REL.DS_OBSERVACAO 
			,REL.ID_OS     
 			,REL.CD_ATIVO_FIXO
			,REL.HR_FIM
			,REL.HR_INICIO
 			,MODE.DS_MODELO  
 			,TEC.VL_CUSTO_HORA
			,REL.CD_TIPO_OS 
 			,PEC.CD_PECA
 			,PEC.DS_PECA 
			,RPE.ID_PECA_OS                    
			,RPE.QT_PECA
			,RPE.VL_VALOR_PECA
			,ContaPecaOS.ContaPeca
			,usu.cnmNome
			,rpe.CD_TP_ESTOQUE_CLI_TEC



END 

--select * from tbOSPadrao

--drop table #T_RelManut_ContaPecaOS

--select * from TB_TECNICO where CD_TECNICO = '347838'