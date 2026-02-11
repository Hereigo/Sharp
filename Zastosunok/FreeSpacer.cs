namespace Zastosunok;

public partial class Form1
{
    private long GetFreeSpaceInGb(string driveLetter)
    {
        try
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.Name == driveLetter)
                {
                    long freeSpaceInBytes = drive.AvailableFreeSpace;
                    long freeSpaceInGb = freeSpaceInBytes / (1024 * 1024 * 1024);
                    return freeSpaceInGb;
                }
            }
            return -1;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting free space: {ex.Message}");
            return -1; // Return -1 to indicate an error
        }
    }
}