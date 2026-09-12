-- =============================================================================
-- Phase 2 Database Migration: Decoupled Multi-Service Relational Schema
-- Target Database: MySQL 8.0 / Pomelo EF Core
-- Safe and Idempotent: Can be run multiple times without data loss
-- =============================================================================

USE devops_deployments;

-- 1. Create Projects table if it does not exist
CREATE TABLE IF NOT EXISTS `Projects` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `Name` VARCHAR(100) NOT NULL,
    `RepositoryUrl` VARCHAR(255) NOT NULL,
    `Description` VARCHAR(500) NOT NULL,
    `CreatedAt` DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 2. Seed initial baseline projects if table is empty
INSERT INTO `Projects` (`Id`, `Name`, `RepositoryUrl`, `Description`, `CreatedAt`)
SELECT 1, 'dotnet-devops-release-tracker', 'https://github.com/MarzanulHoque/dotnet-devops-release-tracker', 'Core DevOps Release Tracker application and deployment telemetry engine.', NOW()
WHERE NOT EXISTS (SELECT 1 FROM `Projects` WHERE `Id` = 1);

INSERT INTO `Projects` (`Id`, `Name`, `RepositoryUrl`, `Description`, `CreatedAt`)
SELECT 2, 'microservices-gateway', 'https://github.com/MarzanulHoque/microservices-gateway', 'Edge API gateway handling routing, authentication, and rate limiting.', NOW()
WHERE NOT EXISTS (SELECT 1 FROM `Projects` WHERE `Id` = 2);

-- 3. Idempotently add ProjectId column to Deployments table if not present
SET @dbname = DATABASE();
SET @tablename = 'Deployments';
SET @columnname = 'ProjectId';
SET @preparedStatement = (SELECT IF(
  (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE
      (TABLE_SCHEMA = @dbname)
      AND (TABLE_NAME = @tablename)
      AND (COLUMN_NAME = @columnname)
  ) > 0,
  'SELECT 1',
  'ALTER TABLE `Deployments` ADD COLUMN `ProjectId` INT NULL, ADD CONSTRAINT `FK_Deployments_Projects_ProjectId` FOREIGN KEY (`ProjectId`) REFERENCES `Projects`(`Id`) ON DELETE SET NULL;'
));
PREPARE alterIfNotExists FROM @preparedStatement;
EXECUTE alterIfNotExists;
DEALLOCATE PREPARE alterIfNotExists;

-- Backfill existing Phase 1 records to baseline project
UPDATE `Deployments` SET `ProjectId` = 1 WHERE `ProjectId` IS NULL AND `ProjectName` = 'dotnet-devops-release-tracker';

-- 4. Create DeploymentLogs table if it does not exist
CREATE TABLE IF NOT EXISTS `DeploymentLogs` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `DeploymentRecordId` INT NOT NULL,
    `Timestamp` DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    `LogLevel` VARCHAR(20) NOT NULL,
    `Message` VARCHAR(1000) NOT NULL,
    `Source` VARCHAR(100) NOT NULL,
    PRIMARY KEY (`Id`),
    KEY `IX_DeploymentLogs_DeploymentRecordId` (`DeploymentRecordId`),
    CONSTRAINT `FK_DeploymentLogs_Deployments_DeploymentRecordId` FOREIGN KEY (`DeploymentRecordId`) REFERENCES `Deployments` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
