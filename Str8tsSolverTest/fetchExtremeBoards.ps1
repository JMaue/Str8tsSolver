$chromePath = "C:\Program Files\Google\Chrome\Application\chrome.exe"
$baseUrl = "https://www.str8ts.com/Players/ASStr8tsASPX.aspx"
$outputBasePath = "D:\repos\MyStr8ts\Str8tsSolver\Str8tsSolverTest\Extreme"
$targetBasePath = ".\WeeklyExtremes"

# https://www.str8ts.com/Players/ASStr8tsASPX.aspx?wp=775
function Get-Str8tsBoard {
    param (
        [Parameter(Mandatory = $true)]
        [string]$FilePath
    )

    # Read the HTML content
    $htmlContent = Get-Content -Path $FilePath -Raw

    # Create HTML object
    $html = New-Object -Com "HTMLFile"
    $html.IHTMLDocument2_write($htmlContent)
 
    # Find the board table
    $boardTable = $html.getElementById("boardtable")
    if ($null -eq $boardTable) {
        Write-Error "Board table not found in HTML"
        return $null
    }
 
    # Initialize 9x9 array for the board
    $board = New-Object 'char[,]' 9,9
 
    # Get all cells from the table
    $cells = $boardTable.getElementsByTagName("td")
    $row = 0
    $col = 0

    foreach ($cell in $cells) {
        if ($cell.id -match "C(\d)(\d)") {
            $row = [int]$Matches[1]
            $col = [int]$Matches[2]
            $value = ' '
            if ($null -ne $cell.innerText) {
              $value = $cell.innerText.Trim()
            }
           
            # Convert empty cells to spaces
            if ([string]::IsNullOrEmpty($value)) {
                $value = ' '
            }
           
            # Handle black cells with numbers (convert to letters A-I)
            if ($cell.className -match "CellBlack") {
                if ($value -eq " ") {
                    $value = '#'
                }
                else {
                    $value = [char]([int][char]'A' + [int]$value - 1)
                }
            }
           
            $board[$row,$col] = $value
        }
    }
 
    # Release COM object
    [System.Runtime.Interopservices.Marshal]::ReleaseComObject($html) | Out-Null
 
    return $board
}
 
# Example usage:
# $board = Get-Str8tsBoard -FilePath "$outputBasePath\2024-03-10.txt"
# Display the board
function Show-Str8tsBoard {
    param (
        [Parameter(Mandatory = $true)]
        [char[]]$board
    )
   
    for ($i = 0; $i -lt 9; $i++) {
        $row = ""
        for ($j = 0; $j -lt 9; $j++) {
            $x = $board[$i*9+$j]
            $row += $x
        }
        Write-Output $row
    }
}
function Write-BoardToFile {
    param (
        [string[]]$board,
        [string]$targetFilePath
    )

    $boardText = @()
    for ($i = 0; $i -lt 9; $i++) {
        $row = ""
        for ($j = 0; $j -lt 9; $j++) {
            $c = $board[$i*9+$j]
            if ($c -eq ' ') {
                $c = '.'
            }
            $row += $c
        }
        $boardText += $row
    }
    $boardText | Out-File -FilePath $targetFilePath
}

for ($d = 775; $d -lt 776; $d++) {

    Write-Output $d
 
    & $chromePath --headless --dump-dom --disable-gpu --virtual-time-budget=10000  "$baseUrl`?wp=$d" | Set-Content -Path "$outputBasePath\$d.txt"
 
    $board = Get-Str8tsBoard -FilePath "$outputBasePath\$d.txt"

    Show-Str8tsBoard -board $board

    # Save board to file
    Write-BoardToFile -board $board -targetFilePath "$targetBasePath\board_$d.txt"
   # $boardText | Out-File -FilePath "$targetBasePath\board_$d.txt"
}
 
Write-Output "Done"
