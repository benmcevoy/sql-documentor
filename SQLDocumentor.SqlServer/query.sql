
	
	set nocount on
	
--temporary tables
	declare @database table ([type] varchar(255), [name] varchar(255), [summary] varchar(4000), [owner] varchar(50), [created] varchar(11), [dbid] int, [cmptlvl] int, [size] varchar(20), server varchar(50) )
	declare @xType table ([xType] char(2), [Description] varchar(50))
	declare @object table ([object_id] int, [type] varchar(255), [name] varchar(255), [summary] varchar(3000), [SQL] varchar(4000) )
	declare @parameters table (object_id int, column_id int, [otype] varchar(255), [oname] varchar(255), [type] varchar(50), [name] varchar(255), [datatype] varchar(255), [size] int, [isNullable] bit, [summary] varchar(4000), [isPK] bit, [isFK] bit, [FKTableName] varchar(255))
	declare @summary table ([otype] varchar(255), [oname] varchar(255), [summary] varchar(1000))

	declare @type varchar(50)
	declare @name varchar(50)
	declare @otype varchar(50)
	declare @oname varchar(50)	
	declare @objectId int
	declare @referencedObjectId int
	declare @referencedColumnId int
	
	declare @comment varchar(4000)

--database info cannabilized from sp_helpdb
	declare @owner varchar(50)
	declare @size varchar(50)
	declare @created varchar(11)

	select 	@owner = suser_sname(sid), 
		@created = convert(varchar(11), crdate)
	from master.dbo.sysdatabases
	where name = db_name()

	insert @database 
	select 	'Database',
		name, 
		'' [summary],
		@owner, 
		@created,
		dbid, 
		cmptlevel ,
		'' [size],
		CONVERT(varchar(50), SERVERPROPERTY('servername'))
	from master.dbo.sysdatabases
	where name = db_name()

	set @size = (select str(convert(dec(15),sum(size))* 8192/ 1048576,10,2)+ ' MB' [size] from sysfiles)

	update @database set [size] = @size

	declare @dbdesc varchar(340)

	-- These properties always available
	SELECT @dbdesc = 'Status=' + convert(sysname,DatabasePropertyEx(db_name(),'Status'))
	SELECT @dbdesc = @dbdesc + ' ' + char(13) + 'Updateability=' + convert(sysname,DatabasePropertyEx(db_name(),'Updateability'))
	SELECT @dbdesc = @dbdesc + ' ' + char(13) + 'UserAccess=' + convert(sysname,DatabasePropertyEx(db_name(),'UserAccess'))
	SELECT @dbdesc = @dbdesc + ' ' + char(13) + 'Recovery=' + convert(sysname,DatabasePropertyEx(db_name(),'Recovery'))
	SELECT @dbdesc = @dbdesc + ' ' + char(13) + 'Version=' + convert(sysname,DatabasePropertyEx(db_name(),'Version'))

	-- These props only available if db not shutdown
	IF DatabaseProperty(db_name(), 'IsShutdown') = 0
	BEGIN
		SELECT @dbdesc = @dbdesc + ' ' + char(13) + 'Collation=' + convert(sysname,DatabasePropertyEx(db_name(),'Collation'))
		SELECT @dbdesc = @dbdesc + ' ' + char(13) + 'SQLSortOrder=' + convert(sysname,DatabasePropertyEx(db_name(),'SQLSortOrder'))
	END

	-- These are the boolean properties
	IF DatabasePropertyEx(db_name(),'IsAutoClose') = 1
		SELECT @dbdesc = @dbdesc + ' ' + char(13) + '' + 'IsAutoClose'
	IF DatabasePropertyEx(db_name(),'IsAutoShrink') = 1
		SELECT @dbdesc = @dbdesc + ' ' + char(13) + '' + 'IsAutoShrink'
	IF DatabasePropertyEx(db_name(),'IsInStandby') = 1
		SELECT @dbdesc = @dbdesc + ' ' + char(13) + '' + 'IsInStandby'
	IF DatabasePropertyEx(db_name(),'IsTornPageDetectionEnabled') = 1
		SELECT @dbdesc = @dbdesc + ' ' + char(13) + '' + 'IsTornPageDetectionEnabled'
	IF DatabasePropertyEx(db_name(),'IsAnsiNullDefault') = 1
		SELECT @dbdesc = @dbdesc + ' ' + char(13) + '' + 'IsAnsiNullDefault'
	IF DatabasePropertyEx(db_name(),'IsAnsiNullsEnabled') = 1
		SELECT @dbdesc = @dbdesc + ' ' + char(13) + '' + 'IsAnsiNullsEnabled'
	IF DatabasePropertyEx(db_name(),'IsAnsiPaddingEnabled') = 1
		SELECT @dbdesc = @dbdesc + ' ' + char(13) + '' + 'IsAnsiPaddingEnabled'
	IF DatabasePropertyEx(db_name(),'IsAnsiWarningsEnabled') = 1
		SELECT @dbdesc = @dbdesc + ' ' + char(13) + '' + 'IsAnsiWarningsEnabled'
	IF DatabasePropertyEx(db_name(),'IsArithmeticAbortEnabled') = 1
		SELECT @dbdesc = @dbdesc + ' ' + char(13) + '' + 'IsArithmeticAbortEnabled'
	IF DatabasePropertyEx(db_name(),'IsAutoCreateStatistics') = 1
		SELECT @dbdesc = @dbdesc + ' ' + char(13) + '' + 'IsAutoCreateStatistics'
	IF DatabasePropertyEx(db_name(),'IsAutoUpdateStatistics') = 1
		SELECT @dbdesc = @dbdesc + ' ' + char(13) + '' + 'IsAutoUpdateStatistics'
	IF DatabasePropertyEx(db_name(),'IsCloseCursorsOnCommitEnabled') = 1
		SELECT @dbdesc = @dbdesc + ' ' + char(13) + '' + 'IsCloseCursorsOnCommitEnabled'
	IF DatabasePropertyEx(db_name(),'IsFullTextEnabled') = 1
		SELECT @dbdesc = @dbdesc + ' ' + char(13) + '' + 'IsFullTextEnabled'
	IF DatabasePropertyEx(db_name(),'IsLocalCursorsDefault') = 1
		SELECT @dbdesc = @dbdesc + ' ' + char(13) + '' + 'IsLocalCursorsDefault'
	IF DatabasePropertyEx(db_name(),'IsNullConcat') = 1
		SELECT @dbdesc = @dbdesc + ' ' + char(13) + '' + 'IsNullConcat'
	IF DatabasePropertyEx(db_name(),'IsNumericRoundAbortEnabled') = 1
		SELECT @dbdesc = @dbdesc + ' ' + char(13) + '' + 'IsNumericRoundAbortEnabled'
	IF DatabasePropertyEx(db_name(),'IsQuotedIdentifiersEnabled') = 1
		SELECT @dbdesc = @dbdesc + ' ' + char(13) + '' + 'IsQuotedIdentifiersEnabled'
	IF DatabasePropertyEx(db_name(),'IsRecursiveTriggersEnabled') = 1
		SELECT @dbdesc = @dbdesc + ' ' + char(13) + '' + 'IsRecursiveTriggersEnabled'
	IF DatabasePropertyEx(db_name(),'IsMergePublished') = 1
		SELECT @dbdesc = @dbdesc + ' ' + char(13) + '' + 'IsMergePublished'
	IF DatabasePropertyEx(db_name(),'IsPublished') = 1
		SELECT @dbdesc = @dbdesc + ' ' + char(13) + '' + 'IsPublished'
	IF DatabasePropertyEx(db_name(),'IsSubscribed') = 1
		SELECT @dbdesc = @dbdesc + ' ' + char(13) + '' + 'IsSubscribed'
	IF DatabasePropertyEx(db_name(),'IsSyncWithBackup') = 1
		SELECT @dbdesc = @dbdesc + ' ' + char(13) + '' + 'IsSyncWithBackup'

	update @database set [summary] = @dbdesc

-- object types
-- AF = Aggregate function (CLR)
-- C = CHECK constraint
-- D = DEFAULT (constraint or stand-alone)
-- F = FOREIGN KEY constraint
-- FN = SQL scalar function
-- FS = Assembly (CLR) scalar-function
-- FT = Assembly (CLR) table-valued function
-- IF = SQL inline table-valued function
-- IT = Internal table
-- P = SQL Stored Procedure
-- PC = Assembly (CLR) stored-procedure
-- PG = Plan guide
-- PK = PRIMARY KEY constraint
-- R = Rule (old-style, stand-alone)
-- RF = Replication-filter-procedure
-- S = System base table
-- SN = Synonym
-- SQ = Service queue
-- TA = Assembly (CLR) DML trigger
-- TF = SQL table-valued-function
-- TR = SQL DML trigger
-- TT = Table type
-- U = Table (user-defined)
-- UQ = UNIQUE constraint
-- V = View
-- X = Extended stored procedure

	insert into @xType values('FN', 'Function')
	insert into @xType values('P', 'Procedure')
	insert into @xType values('U', 'Table')
	insert into @xType values('V', 'View')
	insert into @xType values('CO', 'Column')
	insert into @xType values('DB', 'Database')
--	insert into @xType values('D', 'Default constraint')
	insert into @xType values('F', 'Foreign key constraint')
	insert into @xType values('IF', 'Inlined table-function')
	insert into @xType values('PK', 'Primary key constraint')
	insert into @xType values('TF', 'Table function')
	insert into @xType values('TR', 'Trigger')
--	insert into @xType values('UQ', 'Unique constraint')
--	insert into @xType values('X', 'Extended stored procedure')
--	insert into @xType values('RF', 'Replication filter stored procedure')
--	insert into @xType values('S', 'System table')
--	insert into @xType values('L', 'Log')
--	insert into @xType values('C', 'CHECK constraint')

-- parameters
	insert into @parameters 
	select sc.object_id, 
		sc.column_id,
	 	type.[Description] [oType], 
		so.[name] [oname], 
		'Column', 
		sc.[name], 
		st.name [datatype], 
		case st.user_type_id
			when 231 then sc.max_length/2 --nvarchar
			when 167 then sc.max_length -- varchar
			when 239 then sc.max_length/2 --nchar
			when 175 then sc.max_length -- char
			else null
		end [size],
		sc.is_nullable [isNullable],
		null [summary], 
		(select top 1 si.is_primary_key 
			from sys.indexes si 
			join sys.index_columns sic on sic.index_id = si.index_id 
				and sic.object_id = si.object_id
				and sic.object_id = sc.object_id 
			where sic.index_column_id = sc.column_id and sic.column_id = sc.column_id) [isPK], 
		0 [isFK],
		null [FKTableName]

	from sys.objects so
	join sys.columns sc on sc.object_id = so.object_id
	join sys.types st on st.user_type_id = sc.user_type_id
	join @xType type on type.xType = so.type collate Latin1_General_CI_AS_KS_WS
	
-- objects
	insert into @object
	select 	[name].id,
		type.[Description], 
		[name].[Name],
		null [summary],
		sc.[text] [SQL]
		--null [SQL]
	from sysobjects [name]
	join @xType type on type.xType = [name].xType
	left outer join syscomments sc on sc.id = [name].id
	where ([name].xType in ('U', 'P', 'V', 'FN', 'CO')
		or left([name].[Name],2) != 'dt')
	and [name].[Name] not in ('sysconstraints', 'syssegments')
	
-- summary for parent objects - extended properties
	declare xSum_cursor cursor for 
	select type, [name]
	from @object
	
	open xSum_cursor
	fetch next from xSum_cursor into @type, @name
	
	while @@FETCH_STATUS = 0 begin
	
		update @object set summary = (select convert(varchar(4000), value) 
				from ::fn_listextendedproperty(NULL, 'schema', 'dbo', @type, @name, NULL, NULL))
		where type = @type and name = @name
	
	fetch next from xSum_cursor into @type, @name
	end
	
	close xSum_cursor
	deallocate xSum_cursor

-- summary for parameters/table columns - extended properties
	declare xSum_cursor cursor for 
	select type, [name], otype, oname
	from @parameters
	
	open xSum_cursor
	fetch next from xSum_cursor into @type, @name, @otype, @oname
	
	while @@FETCH_STATUS = 0 begin
		
		update @parameters set summary = (select convert(varchar(4000), value) 
				from ::fn_listextendedproperty(NULL, 'schema', 'dbo', @otype, @oname, @type, @name))
		where type= @type 
		and [name] = @name 
		and otype = @otype 
		and oname = @oname

	fetch next from xSum_cursor into @type, @name, @otype, @oname
	end
	
	close xSum_cursor
	deallocate xSum_cursor

-- summary from comment block if no extended properties
	declare xSum_cursor cursor for 
	select type, [name]
	from @object
	
	open xSum_cursor
	fetch next from xSum_cursor into @type, @name
	
	while @@FETCH_STATUS = 0 begin

		set @comment = ''

		select @comment =  substring(c.[text], patindex('%/*%',c.[text]) + len('/*'), (patindex('%*/%',c.[text]) - patindex('%/*%',c.[text]))) 
		from syscomments c
		join sysobjects o on o.id = c.id
		join @xType type on type.xType = o.xType
		where type.[Description] = @type 
		and o.name = @name

		update @object set summary = @comment
		where type = @type 
		and [name] = @name 
		and summary is null

	fetch next from xSum_cursor into @type, @name
	end
	
	close xSum_cursor
	deallocate xSum_cursor

-- ASSIGN REFERNCE TABLE NAME FOR FOREIGN KEYS
	declare xSum_cursor cursor for 
	select [object_id], [name]
	from @object
	where [type] = 'Table'
	
	open xSum_cursor
	fetch next from xSum_cursor into @objectId, @name
	
	while @@FETCH_STATUS = 0 begin

		declare fk_cursor cursor for 
		select referenced_object_id, parent_column_id from sys.foreign_key_columns fk  
		where fk.parent_object_id = @objectId 
		
		open fk_cursor
		fetch next from fk_cursor into @referencedObjectId, @referencedColumnId
		
		while @@FETCH_STATUS = 0 begin
			
			update @parameters set [FKTableName] = (select name from sys.tables where object_id = @referencedObjectId),
				IsFK = 1
			where object_id = @objectId and column_id = @referencedColumnId

			fetch next from fk_cursor into @referencedObjectId, @referencedColumnId
		end
	
		close fk_cursor
		deallocate fk_cursor
	
	fetch next from xSum_cursor into @objectId, @name
	end
	
	close xSum_cursor
	deallocate xSum_cursor


	insert into @object
	select -1,	'Database', 
		db_name(),
		@dbdesc [summary],
		null [SQL]
	insert into @parameters values(-1, -1, 'Database', db_Name(), 'Column', 'Size', '', null, null, @size, 0, 0, null)
	insert into @parameters values(-1, -1, 'Database', db_Name(), 'Column', 'Owner', '', null, null, @owner, 0, 0, null)
	insert into @parameters values(-1, -1, 'Database', db_Name(), 'Column', 'Created', '', null, null, @created, 0, 0, null)

--select the xml
	select	[database].type, 
			[database].[name], 
			[database].summary, 
			[database].server, 
			objects.type, 
			object.type, 
			object.[name], 
			object.summary, 
			object.SQL,
			param.[name], 
			param.datatype,
			param.[size],
			param.[isNullable],
			param.summary, 
			isnull(param.isPK,0) isPK,
			param.isFK,
			param.fKTableName
	
	from @database [database]
	join @object objects on 1 = 1
	join @object object on objects.type = object.[type] 
		and objects.name = object.[name]
	left outer join @parameters param on param.otype = object.[type] 
		and param.oname = object.[name]
	
	order by object.type, object.[name]
	
	for xml auto
