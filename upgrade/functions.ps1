function prompt { "心莱科技: " + (get-location) + "> "}

<#
.SYNOPSIS
复制目录或者文件，支持过滤

.PARAMETER sPath
源路径

.PARAMETER dPath
目标路径

.PARAMETER filter
【数组】过滤目录或文件

.EXAMPLE
 CopyItemsWithFilter -sPath $sourcePath -dPath $tempPath;
#>
Function  CopyItemsWithFilter($sPath, $dPath, $filter) {
    Write-Host "正在将 $sPath 复制到 $dPath";
    if ([io.Directory]::Exists($dPath)) {
        try {
            Remove-Item $dPath -Recurse  -Force
        }
        catch {
            Write-Error "删除目录出错！$dPath";
            Write-Error $Error[0].Exception;
            return;
        }
    }
    [io.Directory]::CreateDirectory($dPath);
    Set-Location $sPath
    $Cmd = "ROBOCOPY $sPath $dPath /256 /E /XF *.log *.dll *.tmp *.bak *.pubxml /XD bin obj node_modules .cache packages Logs Debug Release";
    Write-Host $Cmd
    cmd /c $Cmd
}
<#
.SYNOPSIS
判断当前变量是否为NULL

.PARAMETER objectToCheck
对象

.EXAMPLE
An example
#>
function IsNull($objectToCheck) {
    if ($null -eq $objectToCheck) {
        return $true
    }

    if ($objectToCheck -is [String] -and $objectToCheck -eq [String]::Empty) {
        return $true
    }

    if ($objectToCheck -is [DBNull] -or $objectToCheck -is [System.Management.Automation.Language.NullString]) {
        return $true
    }

    return $false
}
<#
.SYNOPSIS
复制目录或者文件

.PARAMETER path
路径

.PARAMETER destination
目标路径

.EXAMPLE
CopyItems -paths $paths -path -destination $tempPath

#>
function CopyItems ($path, $destination) {
    Write-Host "正在将 $path 复制到 $destination";
    if ([io.Directory]::Exists($path)) {
        Copy-Item -Path  $path  -Destination $destination  -Recurse  -Force
    }
    elseif ([io.File]::Exists($path)) {
        Copy-Item -Path  $path  -Destination $destination  -Force
    }
}
<#
.SYNOPSIS
移除目录或文件

.PARAMETER path
路径
#>
function RemoveItems($path) {
    Write-Host "移除$path";
    if ([io.Directory]::Exists($path)) {
        Remove-Item $path -Recurse  -Force
    }
    elseif ([io.File]::Exists($path)) {
        Remove-Item $path  -Force
    }
}

<#
.SYNOPSIS
根据指定路径列表复制指定目录或文件

.DESCRIPTION
根据指定路径列表复制指定目录或文件

.PARAMETER paths
路径列表

.PARAMETER path
源路径

.PARAMETER destination
目标路径

.EXAMPLE
An example

.NOTES
General notes
#>
function CopyItemsByPaths($paths, $path, $destination) {
    foreach ($item in $paths) {
        $sPath = [io.Path]::Combine($path, $item);
        if ([io.Directory]::Exists($sPath) -or [io.File]::Exists($sPath)) {
            $dPath = [io.Path]::Combine($destination, [io.Path]::GetDirectoryName($item));
            CopyItems -path $sPath -destination $dPath
        }
    }
}

function RemoveItemsByPaths($paths, $path) {
    foreach ($item in $paths) {
        $sPath = [io.Path]::Combine($path, $item);
        RemoveItems -path $sPath
    }
}