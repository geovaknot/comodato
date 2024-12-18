GO
/****** Object:  StoredProcedure [dbo].[prcRptAgendaSelectAtendimento]    Script Date: 08/11/2021 08:45:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[prcRptAgendaSelectAtendimento]
(
	@p_CDS_TECNICO				VARCHAR(MAX)	= NULL,
	@p_STS_TP_STATUS_VISITA_OS	VARCHAR(MAX)	= NULL,
	@p_nidUsuario	numeric		(18),
	@pDtInicial DAtetime = null,
	@pDtFinal DAtetime = null
)
AS
BEGIN

	SET NOCOUNT ON;
	DECLARE @cdsErrorMessage	NVARCHAR(4000),
		@nidErrorSeverity	INT,
		@nidErrorState		INT,
		@ID_AGENDA			BIGINT,
		@v_CD_TECNICO		VARCHAR(6),
		@v_NM_TECNICO		VARCHAR(50)

	BEGIN TRY

		DECLARE @T_RET TABLE 
		(
			CD_TECNICO				VARCHAR(06)		NULL,
			NM_TECNICO				VARCHAR(50)		NULL,
			CD_CLIENTE				NUMERIC(6,0)	NULL,
			NM_CLIENTE				VARCHAR(50)		NULL,
			EN_CIDADE				VARCHAR(50)		NULL,
			EN_ESTADO				VARCHAR(03)		NULL,
			CD_REGIAO				VARCHAR(02)		NULL,
			DS_REGIAO				VARCHAR(30)		NULL,
			DT_DESATIVACAO			DATETIME		NULL,
			CD_TECNICO_PRINCIPAL	VARCHAR(06)		NULL,
			NM_TECNICO_PRINCIPAL	VARCHAR(50)		NULL,
			QT_PERIODO				INT				NULL,
			CD_ORDEM				INT				NULL,
			ID_OS					bigint,
			ST_STATUS_OS			int,
			DT_DATA_OS				datetime,
			DS_STATUS_OS			varchar(20),
			ID_AGENDA				bigint
		)

		DECLARE db_cursor CURSOR FOR 
			SELECT	
				dbo.TB_Tecnico.CD_TECNICO,
				dbo.TB_Tecnico.NM_TECNICO
			FROM	
				TB_Tecnico (NOLOCK)
			WHERE 
				(@p_CDS_TECNICO IS NULL OR TB_Tecnico.CD_Tecnico COLLATE Latin1_General_CI_AS IN (SELECT CAST(cdsString AS VARCHAR(06)) FROM fncGetValuesByString(@p_CDS_TECNICO,',')) )
			AND	TB_Tecnico.FL_Ativo	= 'S'
			ORDER BY 
				TB_Tecnico.NM_Tecnico	


		OPEN db_cursor  
		FETCH NEXT FROM db_cursor INTO @v_CD_TECNICO, @v_NM_TECNICO  

		WHILE @@FETCH_STATUS = 0  
		BEGIN  
			INSERT INTO @T_RET 
			(
				CD_CLIENTE,
				NM_CLIENTE,
				EN_CIDADE,
				EN_ESTADO,
				CD_REGIAO,
				DS_REGIAO,
				DT_DESATIVACAO,
				CD_TECNICO_PRINCIPAL,
				NM_TECNICO_PRINCIPAL,
				QT_PERIODO,
				CD_ORDEM,
				ID_OS,
				ST_STATUS_OS,
				DT_DATA_OS,
				DS_STATUS_OS,
				ID_AGENDA
			)
			EXEC prcAgendaSelectAtendimento 
				@p_CD_TECNICO	= @v_CD_TECNICO, 
				@p_nidUsuario	= @p_nidUsuario,
				@pDtInicial  = @pDtInicial,
				@pDtFinal = @pDtFinal

			UPDATE @T_RET SET 
				CD_TECNICO	= @v_CD_TECNICO,
				NM_TECNICO	= @v_NM_TECNICO
			WHERE 
				CD_TECNICO IS NULL 

			FETCH NEXT FROM db_cursor INTO @v_CD_TECNICO, @v_NM_TECNICO  
		END 

		CLOSE db_cursor  
		DEALLOCATE db_cursor 


		--SELECT * FROM @T_RET T
		--WHERE (@p_STS_TP_STATUS_VISITA_OS IS NULL OR T.ST_TP_STATUS_VISITA_OS IN (select CAST(cdsString AS INT) from fncGetValuesByString(@p_STS_TP_STATUS_VISITA_OS,',')) )

		-- Correção para filtro por Tipo de Visita
		SELECT * 
		INTO #Temp1
		FROM @T_RET T 
		WHERE ( @p_STS_TP_STATUS_VISITA_OS IS NULL 
		OR T.ID_OS IN (
					
					-- QUERY PARA BUSCAR PELA VISITA INFORMADA
					--SELECT L.ID_VISITA 
					--FROM tbLogStatusVisita L
					--WHERE  L.ID_VISITA = T.ID_VISITA 
					--AND L.ST_TP_STATUS_VISITA_OS IN (select CAST(cdsString AS INT) from fncGetValuesByString(@p_STS_TP_STATUS_VISITA_OS,','))

					-- QUERY PARA BUSCAR PELO STATUS ATUAL DA VISITA (ULTIMO STATUS)
					SELECT L.ID_OS
					FROM dbo.tbLogStatusosPadrao L with(nolock)
					WHERE L.ID_OS = T.ID_OS
					AND L.ID_LOG_STATUS_OS = (SELECT MAX(ID_LOG_STATUS_OS) FROM tbLogStatusOsPadrao with(nolock) WHERE tbLogStatusOsPadrao.ID_OS = L.ID_OS)
					AND L.ST_STATUS_OS IN (select CAST(cdsString AS INT) from fncGetValuesByString(@p_STS_TP_STATUS_VISITA_OS,','))
				)
			)


		SELECT 
				t.CD_CLIENTE,
				NM_CLIENTE + ' (' + Convert(VARCHAR, t.CD_CLIENTE) + ')' AS NM_CLIENTE,
				EN_CIDADE,
				EN_ESTADO,
				CD_REGIAO,
				DS_REGIAO,
				DT_DESATIVACAO,
				CD_TECNICO_PRINCIPAL,
				NM_TECNICO_PRINCIPAL,
				QT_PERIODO,
				CD_ORDEM,
				t.ID_OS,
				t.ST_STATUS_OS,
				t.DT_DATA_OS,
				t.DS_STATUS_OS,
				ID_AGENDA,
				--t.* , 
				m.DS_MODELO AS CD_MODELO, 
				--af.CD_MODELO, 
				m.CD_GRUPO_MODELO, 
				tbOSPadrao.CD_ATIVO_FIXO
		FROM #Temp1 t with(nolock)
		Inner join tbOSPadrao on t.ID_OS = tbOSPadrao.ID_OS 
		--INNER JOIN TB_ATIVO_CLIENTE ac ON ac.CD_CLIENTE = t.CD_CLIENTE AND ac.DT_DEVOLUCAO IS NULL
		INNER JOIN TB_ATIVO_FIXO af ON af.CD_ATIVO_FIXO = tbOSPadrao.CD_ATIVO_FIXO
		INNER JOIN TB_MODELO m ON m.CD_MODELO = af.CD_MODELO
		WHERE t.CD_TECNICO = t.CD_TECNICO_PRINCIPAL
		ORDER BY t.CD_TECNICO, t.CD_CLIENTE;



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

--drop table #Temp1