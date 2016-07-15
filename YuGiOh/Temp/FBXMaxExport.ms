fn fileExists fname = (getfiles fname).count != 0
fn strip str = (trimRight (trimLeft str))

-- Dialog Monitor callback function; looks for MXS export related pop-ups and executes specific commands.
fn DialogCallback =
(
	rogueWindows = #( "Biped" )
	
	-- get current popup window
	h = dialogmonitorops.getwindowhandle()
	windowText = UIAccessor.GetWindowText( h )
	format "window = %\n" (windowText)

	for w = 1 to rogueWindows.count do
	(
		if ( windowText == rogueWindows[ w ] ) do
		(
			format "Closing window '%'\n" (windowText)
			UIAccessor.PressDefaultButton()
		)
	)
	
	true
)

pluginManager.loadClass FBXIMP;
pluginManager.loadClass FBXEXP;

openLog "C:/Users/Seth/DOCUME~1/GitHub/YUGIOH~1/YuGiOh/Temp/3dsMax.log" mode:"w"
print "Start of log file."; flushLog();
while true do
(	
	if ( fileExists "C:/Users/Seth/DOCUME~1/GitHub/YUGIOH~1/YuGiOh/Temp/MaxCommandPipe" ) then
	(
		exportNormals = false;
		exportTangents = false;
		bakeAnimations = false;
		useFileUnits = false;
	
		waitpipe = (openFile "C:/Users/Seth/DOCUME~1/GitHub/YUGIOH~1/YuGiOh/Temp/MaxCommandPipe" mode:"r");
		filename = (readLine waitpipe);
		
		for i = 0 to 3 do
		(
			if ( not (eof waitpipe) ) do
			(
				cmd = (readLine waitpipe);
				
				if (cmd == "exportNormals") then
					exportNormals = true;
				else if (cmd == "exportTangents") then
					exportTangents = true;
				else if (cmd == "bake") then
					bakeAnimations = true;
				else if (cmd == "useFileUnits") then
					useFileUnits = true;				
			)
		)
		
		close waitpipe;

		print filename; flushLog();
		if ( (stricmp (getFilenameType filename) ".max") == 0 ) then
		(
			filename = (strip filename);
			
			print "Starting max loading and fbx conversion"; flushLog();

			with defaultAction #logmsg
			(
				ver = maxVersion()
				if ( ver[ 1 ] >= 9000 ) do
				(
					-- register dialog monitor callback notificition and enable
					dialogmonitorops.registernotification DialogCallback id:#QuietWindows
					dialogmonitorops.enabled = true
				)
				
				doExport = loadMaxFile filename useFileUnits:true quiet:true;

				if ( ver[ 1 ] >= 9000 ) do
				(
					-- unregister and disable
					dialogmonitorops.enabled = false
					dialogmonitorops.unregisternotification id:#QuietWindows
				)
				
				if ( doExport ) do
				(
					FBXExporterSetParam "ResetExport";
					if (bakeAnimations) do
					(
						--print "Baking animations"; flushLog();
						FBXExporterSetParam "BakeAnimation" true;
					)
					
					if (exportNormals) do
					(
						--print "Exporting normals per-vertex"; flushLog();
						FBXExporterSetParam "NormalsPerPoly" true;
					)
					
					if (exportTangents) do
					(
						--print "Exporting tangents"; flushLog();
						FBXExporterSetParam "TangentSpaceExport" true;
					)
					
					FBXExporterSetParam "FilterKeyReducer" false;
					FBXExporterSetParam "UpAxis" "Y";
					
					FBXExporterSetParam "ScaleFactor" 1;
					--FBXExporterSetParam "ConvertUnit" "cm";
					
					if (useFileUnits) then
					(
						print "Settings units"; flushLog();
						
						-- Setting units manually, because some FBX 2011 plugins ignore ScaleFactor setting 
						-- and fails to get units automatically. See case 358431. This code should
						-- be removed one the issue is fixed on Autodesk side
						fileUnits = units.SystemType;					
						if (fileUnits == #kilometers) then
							FBXExporterSetParam "ConvertUnit" #km;
						else if (fileUnits == #meters) then
							FBXExporterSetParam "ConvertUnit" #m;
						else if (fileUnits == #centimeters) then
							FBXExporterSetParam "ConvertUnit" #cm;
						else if (fileUnits == #millimeters) then
							FBXExporterSetParam "ConvertUnit" #mm;
						else if (fileUnits == #miles) then
							FBXExporterSetParam "ConvertUnit" #mi;
						else if (fileUnits == #feet) then
							FBXExporterSetParam "ConvertUnit" #ft;
						else if (fileUnits == #inches) then
							FBXExporterSetParam "ConvertUnit" #in;
					)
					else
					(
						FBXExporterSetParam "ConvertUnit" #cm;
					)
					
					print "Before fbx export"; flushLog();
					try
					(
						exportFile "C:/Users/Seth/DOCUME~1/GitHub/YUGIOH~1/YuGiOh/Temp/MaxExportedFBXFile.fbx" #noprompt;
					)
					catch
					(
						format "*** % ***\n" (getCurrentException()); flushLog();
					)
					print "After fbx export"; flushLog();
				)

				deleteFile "C:/Users/Seth/DOCUME~1/GitHub/YUGIOH~1/YuGiOh/Temp/MaxCommandPipe";
				print "Finished max loading and fbx conversion"; flushLog();
			) -- with defaultAction
		)
		else
		(
			--print (stricmp (getFilenameType filename) "max");
			--print filename; flushLog();
			break;
		)
	)
	
	sleep 1
)