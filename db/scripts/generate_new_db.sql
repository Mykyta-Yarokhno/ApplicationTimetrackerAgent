USE [master]
GO

/****** Object:  Database [timetracking.agent]    Script Date: 5/6/2022 10:09:46 PM ******/
DROP DATABASE IF EXISTS [timetracking.agent]
GO

/****** Object:  Database [timetracking.agent]    Script Date: 5/6/2022 10:09:46 PM ******/
CREATE DATABASE [timetracking.agent]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'timetracking.agent', FILENAME = N'D:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\timetracking.agent.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'timetracking.agent_log', FILENAME = N'D:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\timetracking.agent_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [timetracking.agent].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [timetracking.agent] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [timetracking.agent] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [timetracking.agent] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [timetracking.agent] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [timetracking.agent] SET ARITHABORT OFF 
GO

ALTER DATABASE [timetracking.agent] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [timetracking.agent] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [timetracking.agent] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [timetracking.agent] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [timetracking.agent] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [timetracking.agent] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [timetracking.agent] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [timetracking.agent] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [timetracking.agent] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [timetracking.agent] SET  DISABLE_BROKER 
GO

ALTER DATABASE [timetracking.agent] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [timetracking.agent] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [timetracking.agent] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [timetracking.agent] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [timetracking.agent] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [timetracking.agent] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [timetracking.agent] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [timetracking.agent] SET RECOVERY SIMPLE 
GO

ALTER DATABASE [timetracking.agent] SET  MULTI_USER 
GO

ALTER DATABASE [timetracking.agent] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [timetracking.agent] SET DB_CHAINING OFF 
GO

ALTER DATABASE [timetracking.agent] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [timetracking.agent] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO

ALTER DATABASE [timetracking.agent] SET DELAYED_DURABILITY = DISABLED 
GO

ALTER DATABASE [timetracking.agent] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO

ALTER DATABASE [timetracking.agent] SET QUERY_STORE = OFF
GO

ALTER DATABASE [timetracking.agent] SET  READ_WRITE 
GO


USE [timetracking.agent]
GO
/****** Object:  Table [dbo].[app_running_time]    Script Date: 5/6/2022 10:09:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[app_running_time](
	[start_time] [datetime] NOT NULL,
	[finish_time] [datetime] NULL,
	[app_id] [int] NOT NULL,
	[user_id] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[applications]    Script Date: 5/6/2022 10:09:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[applications](
	[id] [int] NOT NULL,
	[name] [varchar](20) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[oses]    Script Date: 5/6/2022 10:09:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[oses](
	[OS] [nchar](10) NOT NULL,
	[user_id] [int] NULL,
	[platform] [nchar](10) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[users]    Script Date: 5/6/2022 10:09:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[users](
	[id] [int] NOT NULL,
	[name] [varchar](20) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[app_running_time]  WITH CHECK ADD  CONSTRAINT [FK_app_running_time_0_0] FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[app_running_time] CHECK CONSTRAINT [FK_app_running_time_0_0]
GO
ALTER TABLE [dbo].[app_running_time]  WITH CHECK ADD  CONSTRAINT [FK_app_running_time_1_0] FOREIGN KEY([app_id])
REFERENCES [dbo].[applications] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[app_running_time] CHECK CONSTRAINT [FK_app_running_time_1_0]
GO
