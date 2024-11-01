GO
/****** Object:  StoredProcedure [dbo].[prcRptGeralManutencao]    Script Date: 24/11/2021 10:12:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



-- ================================================
-- Author:		Alex Natalino / Paulo Rabelo
-- Alter date: 19/08/2019
-- Description:	Relatório Geral de Manutenção
-- ================================================
ALTER PROCEDURE [dbo].[prcRptGeralManutencao]
	@p_DT_INICIAL		DATETIME		= NULL,--'04-01-2020 00:00:00',
	@p_DT_FINAL			DATETIME		= NULL,--'04-30-2020 23:59:00',
	@p_CD_CLIENTES		VARCHAR(MAX)	= NULL, --'3280,',
	@p_CD_TECNICOS		VARCHAR(MAX)	= NULL,
	@p_CD_ATIVOS_FIXOS	VARCHAR(MAX)	= NULL,
	@p_CD_MODELOS		VARCHAR(MAX)	= NULL,
	@p_CD_GRUPOS		VARCHAR(MAX)	= NULL,
	@p_CD_PECAS			VARCHAR(MAX)	= NULL
AS
 
BEGIN

	-- Declaração de Variáveis
	DECLARE @cdsErrorMessage	NVARCHAR(4000),
			@nidErrorSeverity	INT,
			@nidErrorState		INT,
			@CD_TECNICO			VARCHAR(06)

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SET FMTONLY OFF;

	BEGIN TRY


	Declare	@DT_HOJE DATETIME = CONVERT(date, DATEADD(HH,-3,GETUTCDATE())) --Hoje Meia Noite

	Declare	@DT_10Dias DATETIME = DATEADD(day, -1, @DT_HOJE) 

	--EXECUTE [dbo].[prcGravaTempoVisitaOS] null , @DT_10Dias --@DT_HOJE


		If Exists(Select * from tempdb..SysObjects Where Name Like '%T_RelGeralManut_ContaPecaOS' ) drop table #T_RelGeralManut_ContaPecaOS;

		SELECT 
			osPad.ID_OS, IIF(ISNULL(COUNT(tabPecaOS.ID_PECA_OS),1) = 0,1,ISNULL(COUNT(tabPecaOS.ID_PECA_OS),1) ) AS ContaPeca 
		INTO #T_RelGeralManut_ContaPecaOS
		FROM dbo.TB_TECNICO
		LEFT JOIN 
			--(SELECT os.ID_OS, os.CD_TECNICO, os.CD_CLIENTE, os.ST_STATUS_OS   FROM dbo.tbOSPadrao os
			--		INNER JOIN tbLogStatusOsPadrao lsv
			--		ON lsv.ID_OS = os.ID_OS and lsv.ST_STATUS_OS IN (3,5)
			--) 
			tbOSPadrao osPad ON dbo.TB_TECNICO.CD_TECNICO = osPad.CD_TECNICO
			AND osPad.ST_STATUS_OS in (3,5)
		LEFT JOIN dbo.TB_CLIENTE ON osPad.CD_CLIENTE = dbo.TB_CLIENTE.CD_CLIENTE
		--LEFT JOIN dbo.tbOS ON tbVisita.ID_VISITA = dbo.tbOS.ID_VISITA
		LEFT JOIN dbo.tbPecaOS tabPecaOS ON osPad.ID_OS = tabPecaOS.ID_OS
		LEFT JOIN dbo.TB_PECA tabPeca ON tabPecaOS.CD_PECA = tabPeca.CD_PECA
		LEFT JOIN dbo.TB_ATIVO_FIXO ON dbo.TB_ATIVO_FIXO.CD_ATIVO_FIXO = osPad.CD_ATIVO_FIXO
		LEFT JOIN dbo.TB_MODELO ON dbo.TB_MODELO.CD_MODELO = dbo.TB_ATIVO_FIXO.CD_MODELO
		LEFT JOIN dbo.TB_GRUPO ON dbo.TB_CLIENTE.CD_GRUPO = dbo.TB_GRUPO.CD_GRUPO
		WHERE
			TB_TECNICO.FL_ATIVO = 'S'
			AND ( osPad.ST_STATUS_OS IN (3,5) OR osPad.ST_STATUS_OS IS NULL)
			--AND ( dbo.tbOS.ST_TP_STATUS_VISITA_OS = 4 OR dbo.tbOS.ST_TP_STATUS_VISITA_OS IS NULL)		
			AND ( dbo.TB_CLIENTE.CD_CLIENTE	IN (SELECT cdsString FROM dbo.fncGetValuesByString(@p_CD_CLIENTES, ','))		OR @p_CD_CLIENTES		IS NULL )
			AND ( dbo.TB_TECNICO.CD_TECNICO	COLLATE SQL_Latin1_General_CP1_CI_AI	IN (SELECT cdsString FROM dbo.fncGetValuesByString(@p_CD_TECNICOS, ','))		OR @p_CD_TECNICOS		IS NULL )
			AND ( dbo.TB_ATIVO_FIXO.CD_ATIVO_FIXO	COLLATE SQL_Latin1_General_CP1_CI_AI	IN (SELECT cdsString FROM dbo.fncGetValuesByString(@p_CD_ATIVOS_FIXOS, ','))	OR @p_CD_ATIVOS_FIXOS	IS NULL )
			AND ( dbo.TB_MODELO.CD_MODELO		COLLATE SQL_Latin1_General_CP1_CI_AI	IN (SELECT cdsString FROM dbo.fncGetValuesByString(@p_CD_MODELOS, ','))			OR @p_CD_MODELOS		IS NULL )
			AND ( dbo.TB_GRUPO.CD_GRUPO		COLLATE SQL_Latin1_General_CP1_CI_AI	IN (SELECT cdsString FROM dbo.fncGetValuesByString(@p_CD_GRUPOS, ','))			OR @p_CD_GRUPOS			IS NULL )
			AND ( tabPecaOS.CD_PECA		COLLATE SQL_Latin1_General_CP1_CI_AI	IN (SELECT cdsString FROM dbo.fncGetValuesByString(@p_CD_PECAS, ','))			OR @p_CD_PECAS			IS NULL )
			AND (osPad.DT_DATA_OS 	>= @p_DT_INICIAL	OR @p_DT_INICIAL		IS NULL)  
			AND (osPad.DT_DATA_OS 	<= @p_DT_FINAL	OR @p_DT_FINAL			IS NULL )
		GROUP BY
			osPad.ID_OS


		
		SELECT Distinct
			cli.CD_CLIENTE
			,cli.NM_CLIENTE
			,tec.CD_TECNICO
			,tec.NM_TECNICO
			,osPad.CD_ATIVO_FIXO
			,mode.CD_MODELO
			,mode.DS_MODELO
			,GP.CD_GRUPO
			,GP.DS_GRUPO
			,peca.CD_PECA
			,PC.DS_PECA
			,ISNULL(SUM(peca.QT_PECA), 0) AS QT_PECAS
			,ISNULL(SUM((peca.QT_PECA * PC.VL_PECA)), 0) AS VL_PECAS 
			,ISNULL(SUM(Cast(Convert(float,Replace(osPad.HR_FIM,':','.')) - Convert(float,Replace(osPad.HR_INICIO,':','.')) as float)/ISNULL(ContaPecaOS.ContaPeca,1)), 0) AS QT_HORAS
			,ISNULL(SUM((tec.VL_CUSTO_HORA * Cast(Convert(float,Replace(osPad.HR_FIM,':','.')) - Convert(float,Replace(osPad.HR_INICIO,':','.')) as float)/ISNULL(ContaPecaOS.ContaPeca,1))), 0) AS VL_HORAS
			,CAST(ISNULL(COUNT(osPad.CD_ATIVO_FIXO), 0)AS FLOAT)/ISNULL(ContaPecaOS.ContaPeca,1) AS QT_ATIVOS
			, osPad.ID_OS AS VL_VENDAS
			, osPad.DT_DATA_OS
			, 0 AS VL_PERC --não utilizado
			, osPad.ID_OS
		FROM dbo.TB_TECNICO tec
		LEFT JOIN 
			--(SELECT os.ID_OS, os.CD_TECNICO, os.CD_CLIENTE, os.ST_STATUS_OS, lsv.DT_DATA_LOG_OS, os.CD_ATIVO_FIXO , os.HR_FIM, os.HR_INICIO
			--	FROM dbo.tbOSPadrao os
			--		INNER JOIN dbo.tbLogStatusOSPadrao lsv ON lsv.ID_OS = os.ID_OS and lsv.ST_STATUS_OS IN (3,5) WHERE 
			--		(lsv.DT_DATA_LOG_OS 	>= @p_DT_INICIAL	OR @p_DT_INICIAL		IS NULL)  AND 
			--		(lsv.DT_DATA_LOG_OS 	<= @p_DT_FINAL	OR @p_DT_FINAL			IS NULL ) 
			----AND ( lsv.ST_TP_STATUS_VISITA_OS IN (3,4) OR lsv.ST_TP_STATUS_VISITA_OS IS NULL)
			--) 
			tbOSPadrao osPad ON tec.CD_TECNICO = osPad.CD_TECNICO
		LEFT JOIN tbLogStatusOSPadrao logOs on osPad.ID_OS = logOs.ID_OS
			AND ( logOs.ST_STATUS_OS IN (3,5) OR logOs.ST_STATUS_OS IS NULL)
		LEFT JOIN dbo.TB_CLIENTE cli ON osPad.CD_CLIENTE = cli.CD_CLIENTE
		--LEFT JOIN dbo.tbOS ON tbVisita.ID_VISITA = dbo.tbOS.ID_VISITA
		LEFT JOIN dbo.tbPecaOS peca ON osPad.ID_OS = peca.ID_OS
		LEFT JOIN dbo.TB_PECA PC ON peca.CD_PECA = PC.CD_PECA
		LEFT JOIN dbo.TB_ATIVO_FIXO ATV ON ATV.CD_ATIVO_FIXO = osPad.CD_ATIVO_FIXO
		LEFT JOIN dbo.TB_MODELO mode ON mode.CD_MODELO = ATV.CD_MODELO
		LEFT JOIN dbo.TB_GRUPO GP ON cli.CD_GRUPO = GP.CD_GRUPO
		LEFT JOIN #T_RelGeralManut_ContaPecaOS ContaPecaOS ON ContaPecaOS.ID_OS = osPad.ID_OS
		WHERE
			tec.FL_ATIVO = 'S'
			AND ( osPad.ST_STATUS_OS IN (3,5) OR osPad.ST_STATUS_OS IS NULL)
			--AND ( dbo.tbOS.ST_TP_STATUS_VISITA_OS = 4 OR dbo.tbOS.ST_TP_STATUS_VISITA_OS IS NULL)	
			AND (osPad.ID_OS IS NOT NULL)
			AND ( cli.CD_CLIENTE	IN (SELECT cdsString FROM dbo.fncGetValuesByString(@p_CD_CLIENTES, ','))		OR @p_CD_CLIENTES		IS NULL )
			AND ( tec.CD_TECNICO	COLLATE SQL_Latin1_General_CP1_CI_AI	IN (SELECT cdsString FROM dbo.fncGetValuesByString(@p_CD_TECNICOS, ','))		OR @p_CD_TECNICOS		IS NULL )
			AND ( ATV.CD_ATIVO_FIXO	COLLATE SQL_Latin1_General_CP1_CI_AI	IN (SELECT cdsString FROM dbo.fncGetValuesByString(@p_CD_ATIVOS_FIXOS, ','))	OR @p_CD_ATIVOS_FIXOS	IS NULL )
			AND ( mode.CD_MODELO		COLLATE SQL_Latin1_General_CP1_CI_AI	IN (SELECT cdsString FROM dbo.fncGetValuesByString(@p_CD_MODELOS, ','))			OR @p_CD_MODELOS		IS NULL )
			AND ( GP.CD_GRUPO		COLLATE SQL_Latin1_General_CP1_CI_AI	IN (SELECT cdsString FROM dbo.fncGetValuesByString(@p_CD_GRUPOS, ','))			OR @p_CD_GRUPOS			IS NULL )
			AND ( peca.CD_PECA		COLLATE SQL_Latin1_General_CP1_CI_AI	IN (SELECT cdsString FROM dbo.fncGetValuesByString(@p_CD_PECAS, ','))			OR @p_CD_PECAS			IS NULL )
			AND (osPad.DT_DATA_OS 	>= @p_DT_INICIAL	OR @p_DT_INICIAL		IS NULL)  
			AND (osPad.DT_DATA_OS 	<= @p_DT_FINAL	OR @p_DT_FINAL			IS NULL )
		GROUP BY
			osPad.ID_OS
			,osPad.CD_CLIENTE
			,cli.CD_CLIENTE
			,cli.NM_CLIENTE
			,tec.CD_TECNICO
			,tec.NM_TECNICO
			,osPad.CD_ATIVO_FIXO
			--,dbo.tbOS.ID_OS
			,mode.CD_MODELO
			,mode.DS_MODELO
			,GP.CD_GRUPO
			,GP.DS_GRUPO
			,peca.CD_PECA
			,PC.DS_PECA
			, osPad.DT_DATA_OS
			,ContaPecaOS.ContaPeca
			, osPad.ID_OS
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

--drop table #T_RelGeralManut_ContaPecaOS

--select * from tbOSPadrao where cd_cliente = 3280 and dt_data_os >= '04-01-2020 00:00:00' and dt_data_os <= '04-30-2020 23:59:00'

--select * from tb_tecnico where cd_tecnico = '347802'

--select * from tbOSPadrao where id_os in(272242,272243,272241,272244,272245,272246,272240,272239)

--select * from #T_RelGeralManut_ContaPecaOS

--select * from tbPecaOS
