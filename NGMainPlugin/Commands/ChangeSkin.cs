namespace NGMainPlugin.Commands
{
    using CommandSystem;
    using Exiled.API.Features;
    using MapEditorReborn.API.Features;
    using MapEditorReborn.API.Features.Objects;
    using MapEditorReborn.API.Features.Serializable;
    using System;
    using Exiled.API.Enums;
    using Exiled.Permissions.Extensions;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Transform : ICommand
    {
        public string Command => "trnasform";

        public string[] Aliases => new string[] { "trf" };

        public string Description => "Gives a specified player an other skin, while he keeps his role.";

        public bool SanitizeResponse => true;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (player == null)
            {
                response = "Spieler nicht gefunden!";
                return false;
            }

            if (!player.CheckPermission("NG.transform"))
            {
                response = "Dazu hast du keine Rechte!";
                return false;
            }

            Log.Debug($"Received command from player: {player.Nickname}, Arguments: {string.Join(", ", arguments)}");

            if (arguments.Count < 2)
            {
                response = "Bitte geben Sie die Spieler-ID und den Namen des Schematics an, dem das Schematic folgen soll.";
                return false;
            }

            string targetId = arguments.Array[1];
            string schematicName = string.Join(" ", arguments.Array[2]); // Kombiniere alle Teile des Namens

            if (!int.TryParse(targetId, out _))
            {
                response = "Die Spieler-ID muss eine ganze Zahl sein.";
                return false;
            }

            Log.Debug($"Target player ID: {targetId}");

            Player targetPlayer = Player.Get(targetId);

            if (targetPlayer == null)
            {
                response = "Der angegebene Spieler wurde nicht gefunden!";
                return false;
            }

            Log.Debug($"Found target player: {targetPlayer.Nickname}");

            try
            {
                // Schematic erstellen und dem Spieler folgen lassen
                CreateFollowingSchematic(targetPlayer, schematicName);

                response = $"Ein Schematic wurde erstellt, das {targetPlayer.Nickname} verfolgt.";
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred while creating schematic: {ex}");
                response = "Ein Fehler ist aufgetreten.";
                return false;
            }
        }

        private void CreateFollowingSchematic(Player player, string schematicName)
        {
            Log.Debug($"Creating following schematic '{schematicName}' for player: {player.Nickname}");

            // Überprüfen, ob der Spieler noch lebt
            if (!player.IsAlive)
            {
                Log.Error($"Player {player.Nickname} is not alive.");
                return;
            }

            // Überprüfen, ob bereits ein Schematic vorhanden ist
            if (player.GameObject == null)
            {
                Log.Error("Player GameObject is null");
                return;
            }

            if (!player.GameObject.TryGetComponent(out FollowPlayerComponent followPlayerComponent))
            {
                Log.Debug("No followPlayerComponent found. Adding new component.");
                // Schematic über dem Spieler spawnen und ihm folgen lassen
                SchematicObject followingSchematic = ObjectSpawner.SpawnSchematic(new SchematicSerializable()
                {
                    Position = player.Position,
                    Rotation = player.Rotation.eulerAngles, // Spielerrotation übernehmen
                    Scale = UnityEngine.Vector3.one,
                    IsPickable = false,
                    RoomType = RoomType.Surface,
                    SchematicName = schematicName // Hier den Namen des Schematics einfügen
                });

                // Komponente hinzufügen, um dem Spieler zu folgen
                followPlayerComponent = followingSchematic.gameObject.AddComponent<FollowPlayerComponent>();
                followPlayerComponent.TargetPlayer = player;
            }
            else
            {
                Log.Debug("Found existing followPlayerComponent.");
                // Schematic aktualisieren, um einem neuen Spieler zu folgen
                followPlayerComponent.TargetPlayer = player;
            }
        }
    }

    public class FollowPlayerComponent : UnityEngine.MonoBehaviour
    {
        public Player TargetPlayer;

        private void Update()
        {
            if (TargetPlayer != null && TargetPlayer.IsAlive)
            {
                // Schematic dem Spieler folgen lassen
                transform.position = TargetPlayer.Position;
                transform.rotation = TargetPlayer.Rotation; // Spielerrotation übernehmen
            }
            else
            {
                // Wenn der Spieler nicht mehr lebt, das Schematic zerstören
                Destroy(gameObject);
            }
        }
    }
}