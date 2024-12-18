GO
/****** Object:  StoredProcedure [dbo].[prcTecnicoTransferirCarteira]    Script Date: 08/11/2021 14:15:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Alex Natalino
-- Create date: 07/08/2018
-- Description:	Transfere a carteira de clientes em
--              TB_Tecnico_Cliente de um técnico para outro
-- =============================================
ALTER PROCEDURE [dbo].[prcTecnicoTransferirCarteira]	
	@p_CD_TECNICO_ORIGEM		VARCHAR(06) = NULL,
	@p_CD_TECNICO_DESTINO		VARCHAR(06) = NULL,
	@p_nidUsuarioAtualizacao	NUMERIC(18,0)	= NULL
AS
BEGIN

	-- Declaração de Variáveis
	DECLARE @cdsErrorMessage	NVARCHAR(4000),
			@nidErrorSeverity	INT,
			@nidErrorState		INT,
			@nidLog				NUMERIC(18,0),

			@CD_CLIENTE			NUMERIC(06) = NULL,
			@CD_ORDEM			INT			= NULL,
			@ID_AGENDA			BIGINT		= NULL

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY

		BEGIN TRANSACTION
		If Exists(Select * from tempdb..SysObjects Where Name like '%tempTecClienteOrigem' ) drop table #tempTecClienteOrigem;
		If Exists(Select * from tempdb..SysObjects Where Name like '%tempTecClienteDestino' ) drop table #tempTecClienteDestino;
		
		-- Identifica todos os clientes onde o CD_TECNICO (a desativar) esteja com CD_ORDEM = 1
		DECLARE C1 CURSOR LOCAL FORWARD_ONLY FAST_FORWARD FOR
	    SELECT 
			dbo.TB_TECNICO_CLIENTE.CD_CLIENTE
		FROM dbo.TB_TECNICO_CLIENTE with(nolock)
		WHERE	dbo.TB_TECNICO_CLIENTE.CD_TECNICO	= @p_CD_TECNICO_ORIGEM
		AND		dbo.TB_TECNICO_CLIENTE.CD_ORDEM		= 1
		
	    OPEN C1
	    FETCH NEXT FROM C1
			INTO @CD_CLIENTE

	    --WHILE @@FETCH_STATUS = 0
	    BEGIN

			-- Verifica se o CD_TECNICO (destino) já atende este cliente
			IF EXISTS(SELECT * FROM dbo.TB_TECNICO_CLIENTE WHERE CD_CLIENTE = @CD_CLIENTE AND CD_TECNICO = @p_CD_TECNICO_DESTINO)
			BEGIN
				-- Atende
				-- Atualiza o registro do CD_TECNICO (destino) trocando somente o conteúdo de CD_ORDEM para 1
				UPDATE	dbo.TB_TECNICO_CLIENTE 
				SET		CD_ORDEM		= 1
				WHERE	CD_CLIENTE		= @CD_CLIENTE
				AND		CD_TECNICO		= @p_CD_TECNICO_DESTINO

			
				-- Busca a Agenda do CD_TECNICO (a desativar) para ser excluída
				DECLARE C2 CURSOR LOCAL FORWARD_ONLY FAST_FORWARD FOR
				SELECT	
					ID_AGENDA 
				FROM dbo.tbAgenda
				WHERE	CD_CLIENTE	= @CD_CLIENTE
				AND		CD_TECNICO	= @p_CD_TECNICO_ORIGEM

				OPEN C2
				FETCH NEXT FROM C2
					INTO @ID_AGENDA

				WHILE @@FETCH_STATUS = 0
				BEGIN

					EXEC dbo.prcAgendaDelete 
						@p_ID_AGENDA				= @ID_AGENDA,
						@p_nidUsuarioAtualizacao	= @p_nidUsuarioAtualizacao

					FETCH NEXT FROM C2
						INTO @ID_AGENDA;
				END;
				CLOSE C2;
				DEALLOCATE C2;

				-- Exclui o registro do CD_TECNICO (a desativar)
				

				-- Reordena o cliente para corrigir a lacuna da posição trocada do CD_TECNICO (destino)
				EXEC dbo.prcTecnicoClienteReordenar 
				    @p_TP_ACAO = 'R',
				    @p_CD_CLIENTE = @CD_CLIENTE,
				    @p_CD_ORDEM = null,
				    @p_nidUsuarioAtualizacao = @p_nidUsuarioAtualizacao

				-- Gera a Agenda do CD_TECNICO (destino) na CD_ORDEM = 1 
				EXEC dbo.prcAgendaInsert 
					@p_CD_CLIENTE				= @CD_CLIENTE,
					@p_CD_TECNICO				= @p_CD_TECNICO_DESTINO,
					@p_NR_ORDENACAO				= NULL,
					@p_nidUsuarioAtualizacao	= @p_nidUsuarioAtualizacao,
					@p_ID_AGENDA				= @ID_AGENDA OUTPUT

			END
			ELSE
			BEGIN
				-- Não atende
				-- Atualiza o registro do CD_TECNICO (a desativar) trocando somente o contéudo de CD_TECNICO para o CD_TECNICO (destino)
				--UPDATE dbo.TB_TECNICO_CLIENTE
				--SET		CD_TECNICO		= @p_CD_TECNICO_DESTINO
				--WHERE	CD_CLIENTE		= @CD_CLIENTE
				--AND		CD_TECNICO		= @p_CD_TECNICO_ORIGEM

				
				SELECT tecnicoCliente.CD_TECNICO, tecnicoCliente.CD_CLIENTE, tecnicoCliente.CD_ORDEM
				into #tempTecClienteOrigem
					FROM dbo.TB_TECNICO_CLIENTE tecnicoCliente with(nolock)
					WHERE tecnicoCliente.CD_TECNICO = @p_CD_TECNICO_ORIGEM
					Group by tecnicoCliente.CD_CLIENTE, tecnicoCliente.CD_TECNICO, tecnicoCliente.CD_ORDEM
				
				SELECT tecnicoCliente.CD_TECNICO, tecnicoCliente.CD_CLIENTE, tecnicoCliente.CD_ORDEM
				into #tempTecClienteDestino
					FROM dbo.TB_TECNICO_CLIENTE tecnicoCliente with(nolock)
					WHERE tecnicoCliente.CD_TECNICO = @p_CD_TECNICO_DESTINO
					Group by tecnicoCliente.CD_CLIENTE, tecnicoCliente.CD_TECNICO, tecnicoCliente.CD_ORDEM

				IF EXISTS(select tempOrigem.CD_CLIENTE from #tempTecClienteOrigem tempOrigem, #tempTecClienteDestino tempDestino with(nolock)
						  where tempOrigem.CD_CLIENTE = tempDestino.CD_CLIENTE)
				begin
					update dbo.TB_TECNICO_CLIENTE
						set CD_ORDEM = tempOrigem.CD_ORDEM
					from #tempTecClienteOrigem tempOrigem, #tempTecClienteDestino tempDestino with(nolock)
					where dbo.TB_TECNICO_CLIENTE.CD_TECNICO = @p_CD_TECNICO_DESTINO AND dbo.TB_TECNICO_CLIENTE.CD_CLIENTE = tempOrigem.CD_CLIENTE
				end
				else
				begin
					insert into dbo.TB_TECNICO_CLIENTE (CD_CLIENTE, CD_TECNICO, CD_ORDEM)
					select distinct tempOrigem.CD_CLIENTE, tempDestino.CD_TECNICO, tempOrigem.CD_ORDEM
						from #tempTecClienteOrigem tempOrigem, #tempTecClienteDestino tempDestino with(nolock)
					where tempOrigem.CD_CLIENTE not in (select tempDestino.CD_CLIENTE from #tempTecClienteDestino tempDestino)
				end
				
			END
						
		    FETCH NEXT FROM C1
				INTO @CD_CLIENTE;
	    END;
	    CLOSE C1;
	    DEALLOCATE C1;


		-- Identifica todos os clientes onde o CD_TECNICO (a desativar) esteja com CD_ORDEM > 1
		DECLARE C1 CURSOR LOCAL FORWARD_ONLY FAST_FORWARD FOR
	    SELECT 
			dbo.TB_TECNICO_CLIENTE.CD_CLIENTE
		FROM dbo.TB_TECNICO_CLIENTE
		WHERE	dbo.TB_TECNICO_CLIENTE.CD_TECNICO	= @p_CD_TECNICO_ORIGEM
		AND		dbo.TB_TECNICO_CLIENTE.CD_ORDEM		> 1
		
	    OPEN C1
	    FETCH NEXT FROM C1
			INTO @CD_CLIENTE

	    --WHILE @@FETCH_STATUS = 0
	    BEGIN

			-- Exclui o registro do CD_TECNICO (a desativar)
			
			-- Reordena o cliente para corrigir a lacuna da posição excluída do CD_TECNICO (a desativar)
			EXEC dbo.prcTecnicoClienteReordenar 
				@p_TP_ACAO = 'R',
				@p_CD_CLIENTE = @CD_CLIENTE,
				@p_CD_ORDEM = null,
				@p_nidUsuarioAtualizacao = @p_nidUsuarioAtualizacao
						
		    FETCH NEXT FROM C1
				INTO @CD_CLIENTE;
	    END;
	    CLOSE C1;
	    DEALLOCATE C1;

		COMMIT TRANSACTION
		--drop table #tempTecCliente
	
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


select * from #tempTecClienteDestino 
select * from #tempTecClienteOrigem

--drop table #tempTecClienteDestino 
--drop table #tempTecClienteOrigem
