-- Economy Tables for LibreMU
-- Schema: data
-- Run this script once to create the economy tables in the OpenMU PostgreSQL database.

-- 1. EconomyTransaction: Immutable log of all P2P transactions
CREATE TABLE IF NOT EXISTS data."EconomyTransaction" (
    "Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "Timestamp" timestamptz NOT NULL DEFAULT now(),
    "TransactionType" int NOT NULL,
    "SellerId" uuid,
    "BuyerId" uuid,
    "ItemName" varchar(255) NOT NULL,
    "Quantity" int NOT NULL DEFAULT 1,
    "PriceZen" int,
    "PaymentItemName" varchar(255),
    "PaymentItemQuantity" int,
    "CommissionZen" int
);

CREATE INDEX IF NOT EXISTS idx_economytransaction_timestamp ON data."EconomyTransaction"("Timestamp" DESC);
CREATE INDEX IF NOT EXISTS idx_economytransaction_item ON data."EconomyTransaction"("ItemName");

-- 2. MarketPriceSnapshot: Hourly price averages
CREATE TABLE IF NOT EXISTS data."MarketPriceSnapshot" (
    "Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "SnapshotTime" timestamptz NOT NULL DEFAULT now(),
    "ItemName" varchar(255) NOT NULL,
    "ReferenceItemName" varchar(255) NOT NULL DEFAULT 'Jewel of Bless',
    "AveragePrice" decimal(18,4) NOT NULL,
    "TransactionCount" int NOT NULL DEFAULT 0,
    "MinPrice" decimal(18,4) NOT NULL,
    "MaxPrice" decimal(18,4) NOT NULL
);

CREATE INDEX IF NOT EXISTS idx_marketpricesnapshot_time ON data."MarketPriceSnapshot"("SnapshotTime" DESC);
CREATE INDEX IF NOT EXISTS idx_marketpricesnapshot_item ON data."MarketPriceSnapshot"("ItemName", "ReferenceItemName");

-- 3. CharacterPatrimony: Wealth snapshots per character
CREATE TABLE IF NOT EXISTS data."CharacterPatrimony" (
    "Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "SnapshotTime" timestamptz NOT NULL DEFAULT now(),
    "CharacterId" uuid NOT NULL,
    "TotalPatrimony" decimal(18,2) NOT NULL DEFAULT 0,
    "ZenValue" decimal(18,2) NOT NULL DEFAULT 0,
    "BlessCount" int NOT NULL DEFAULT 0,
    "BlessValue" decimal(18,2) NOT NULL DEFAULT 0,
    "SoulCount" int NOT NULL DEFAULT 0,
    "SoulValue" decimal(18,2) NOT NULL DEFAULT 0,
    "LifeCount" int NOT NULL DEFAULT 0,
    "LifeValue" decimal(18,2) NOT NULL DEFAULT 0,
    "ChaosCount" int NOT NULL DEFAULT 0,
    "ChaosValue" decimal(18,2) NOT NULL DEFAULT 0
);

CREATE INDEX IF NOT EXISTS idx_characterpatrimony_character ON data."CharacterPatrimony"("CharacterId");
CREATE INDEX IF NOT EXISTS idx_characterpatrimony_time ON data."CharacterPatrimony"("SnapshotTime" DESC);

-- 4. SystemBank: Singleton commission accumulator
CREATE TABLE IF NOT EXISTS data."SystemBank" (
    "Id" uuid PRIMARY KEY DEFAULT '00000000-0000-0000-0000-000000000001',
    "TotalZenCollected" bigint NOT NULL DEFAULT 0,
    "TotalTransactions" bigint NOT NULL DEFAULT 0,
    "LastUpdated" timestamptz NOT NULL DEFAULT now()
);

-- Insert singleton row if not exists
INSERT INTO data."SystemBank" ("Id", "TotalZenCollected", "TotalTransactions", "LastUpdated")
VALUES ('00000000-0000-0000-0000-000000000001', 0, 0, now())
ON CONFLICT ("Id") DO NOTHING;

-- 5. ReferencePrice: Admin-set reference prices
CREATE TABLE IF NOT EXISTS data."ReferencePrice" (
    "Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "ItemName" varchar(255) NOT NULL UNIQUE,
    "PriceInBless" decimal(18,4) NOT NULL DEFAULT 0,
    "LastUpdated" timestamptz NOT NULL DEFAULT now(),
    "IsActive" boolean NOT NULL DEFAULT true
);

-- Seed initial reference prices (adjust as needed)
INSERT INTO data."ReferencePrice" ("ItemName", "PriceInBless", "LastUpdated")
VALUES
    ('Jewel of Soul', 0.5, now()),
    ('Jewel of Life', 3.0, now()),
    ('Jewel of Chaos', 5.0, now()),
    ('Jewel of Creation', 10.0, now())
ON CONFLICT ("ItemName") DO UPDATE SET "PriceInBless" = EXCLUDED."PriceInBless";
