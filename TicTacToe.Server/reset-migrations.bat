@echo off
echo.
echo Resetting Migrations and Database...
echo ------------------------------------
echo.

echo [Step 1/4] Deleting Migrations folder (if it exists)...
rmdir /s /q Migrations
IF ERRORLEVEL 1 (
    echo [ERROR] Failed to delete the Migrations folder. 
    echo This can happen if the folder exists and is in use or due to permission issues.
    echo Please check manually. Aborting script.
    goto FatalError
)
echo Migrations folder successfully deleted or was not present.
echo.

echo [Step 2/4] Creating initial migration (InitialCreate)...
CALL dotnet ef migrations add InitialCreate
IF ERRORLEVEL 1 (
    echo [ERROR] Failed to create initial migration 'InitialCreate'. Aborting.
    goto FatalError
)
echo Initial migration created successfully.
echo.

echo [Step 3/4] Dropping database...
CALL dotnet ef database drop --force
IF ERRORLEVEL 1 (
    echo [ERROR] Failed to drop database. 
    echo This could be due to connection issues, permissions, or the database being in use.
    echo If the database didn't exist, this step usually succeeds silently with --force.
    echo Aborting script.
    goto FatalError
)
echo Database dropped successfully.
echo.

echo [Step 4/4] Updating database...
CALL dotnet ef database update
IF ERRORLEVEL 1 (
    echo [ERROR] Failed to update database using migrations. Aborting.
    goto FatalError
)
echo Database updated successfully.
echo.

echo ------------------------------------
echo Script completed successfully.
goto EndScript

:FatalError
echo ------------------------------------
echo Script terminated due to an error.
exit /b 1

:EndScript
exit /b 0

