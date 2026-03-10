namespace GameLibrary.Configuration
{
  public class AppConfig
  {
    public required string DownloadPath { get; set; }
    public int MaxDownloadSpeed { get; set; }
    public bool EnableAutoUpdates { get; set; }
    public Theme CurrentTheme { get; set; }

    public enum Theme
    {
      Light,
      Dark,
      System
    }
  }
}