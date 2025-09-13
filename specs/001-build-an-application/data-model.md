# Data Model (Phase 1)

Notation: FieldName (Type) [Constraints]

## PersonProfile
- Id (uuid) [pk]
- ExternalId (text) [unique, nullable]  # Workday ID
- Type (enum: Employee, Contractor, Visitor)
- Status (enum: Active, Suspended, Deactivated)
- DisplayName (text) [not null]
- Email (text) [unique, nullable for visitor]
- Department (text)
- CreatedAt (timestamptz) [default now]
- ModifiedAt (timestamptz)

## Credential
- Id (uuid) [pk]
- PersonProfileId (uuid) [fk→PersonProfile]
- Identifier (text) [unique active constraint]
- Type (enum: Card, Mobile, TempVisitor)
- Status (enum: Active, Revoked, Expired)
- IssuedAt (timestamptz)
- ExpiresAt (timestamptz, nullable)
- RevokedAt (timestamptz, nullable)
- RevocationReason (text, nullable)

Rule: Unique (Identifier) WHERE Status='Active'

## Zone
- Id (uuid) [pk]
- Name (text) [unique]
- Description (text)

## Schedule
- Id (uuid) [pk]
- Name (text) [unique]
- TimeRules (jsonb)  # structured recurrence set
- Timezone (text)

## ZonePermission
- Id (uuid) [pk]
- PersonProfileId (uuid) [fk]
- ZoneId (uuid) [fk]
- ScheduleId (uuid) [fk]
- GrantedAt (timestamptz)
- RevokedAt (timestamptz, nullable)

Composite uniqueness: (PersonProfileId, ZoneId, RevokedAt IS NULL)

## AccessAttempt
- Id (bigserial) [pk]
- CredentialId (uuid) [fk]
- ZoneId (uuid) [fk]
- Outcome (enum: Allow, Deny)
- ReasonCode (text, nullable)
- Timestamp (timestamptz) [index]

## AuditLog
- Id (bigserial) [pk]
- ActorId (uuid, nullable)  # System actions may be null
- EntityType (text)
- EntityId (uuid)
- ActionType (text)
- Data (jsonb)
- CreatedAt (timestamptz) [index]

## AccessTemplate
- Id (uuid) [pk]
- Name (text) [unique]
- Version (int)
- TemplateData (jsonb)  # array of zone+schedule references

## VisitorBadge
- Id (uuid) [pk]
- PersonProfileId (uuid) [fk]
- HostProfileId (uuid) [fk→PersonProfile]
- ExpiresAt (timestamptz)
- Status (enum: Active, Expired, Revoked)

## ReasonCode
- Code (text) [pk]
- Category (text)
- Description (text)
- Active (bool)

## Derived / Behavioral Notes
- Revocation sets Credential.Status=Revoked, Credential.RevokedAt, requires RevocationReason
- Expiration job sets Status=Expired where ExpiresAt < now
- Access Decision Steps:
  1. Find credential (Active)
  2. Confirm profile Status=Active
  3. Check zone permission record with current schedule and time rule evaluation
  4. Log outcome (AccessAttempt)

## Indexing Plan
- AccessAttempt (Timestamp DESC, ZoneId, CredentialId)
- Credential (Identifier)
- ZonePermission (PersonProfileId, ZoneId, RevokedAt)
- AuditLog (EntityType, EntityId, CreatedAt DESC)

## Retention & Archival
- AccessAttempt older than 13 months → export & purge (future phase)

## Open Items
- Mobile credential fields extension (future)
- Partitioning strategy triggers (future)
