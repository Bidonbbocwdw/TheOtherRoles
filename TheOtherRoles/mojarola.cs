using System;
using System.Collections.Generic;
using System.Linq;
using AmongUs.Api;
using UnityEngine;

namespace TheOtherRoles.Roles
{
    public class Transporter : CustomRole
    {
        // 1. Podstawowe ustawienia roli
        public override string Name => "Transporter";
        public override Color Color => new Color(0.1f, 0.7f, 0.9f); // Jasnoniebieski kolor roli
        public override RoleType RoleType => RoleType.Crewmate; // Rola dla załoganta (Crewmate)
        public override string Description => "Możesz teleportować się do najbliższego venta!";

        // Zmienna przechowująca czas odnowienia (cooldown) w sekundach
        private float teleportCooldown = 20f;
        private float currentCooldown = 0f;

        // 2. Co się dzieje na początku rundy
        public override void OnGameStart()
        {
            currentCooldown = teleportCooldown; // Ustawiamy początkowy cooldown
        }

        // 3. Główna logika sprawdzana co klatkę gry (odliczanie cooldownu i teleportacja)
        public override void OnUpdate()
        {
            // Jeśli gra się nie toczy lub nasza postać nie żyje, nic nie rób
            if (PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data.IsDead) return;

            // Odliczanie czasu cooldownu
            if (currentCooldown > 0)
            {
                currentCooldown -= Time.deltaTime;
                // Aktualizujemy napis na przycisku umiejętności, jeśli projekt go wyświetla
                if (Button != null)
                {
                    Button.Timer = currentCooldown;
                }
            }

            // Sprawdzamy, czy gracz nacisnął przycisk umiejętności (w TheOtherRoles domyślnie to przycisk "F" lub specjalny przycisk na ekranie)
            if (Button != null && Button.IsPressed && currentCooldown <= 0)
            {
                TeleportujDoNajblizszegoVenta();
            }
        }

        // 4. Logika wyszukiwania najbliższego venta i teleportacji
        private void TeleportujDoNajblizszegoVenta()
        {
            Vector2 pozycjaGracza = PlayerControl.LocalPlayer.transform.position;
            Vent[] wszystkieVenty = UnityEngine.Object.FindObjectsOfType<Vent>();

            if (wszystkieVenty == null || wszystkieVenty.Length == 0)
            {
                return; // Jeśli na mapie nie ma ventów, nic nie rób
            }

            Vent najblizszyVent = null;
            float najmniejszyDystans = float.MaxValue;

            // Pętla szukająca venta, który jest najbliżej gracza
            foreach (Vent vent in wszystkieVenty)
            {
                float dystans = Vector2.Distance(pozycjaGracza, vent.transform.position);
                if (dystans < najmniejszyDystans)
                {
                    najmniejszyDystans = dystans;
                    najblizszyVent = vent;
                }
            }

            // Jeśli znaleziono najbliższy vent, przenieś tam gracza
            if (najblizszyVent != null)
            {
                PlayerControl.LocalPlayer.transform.position = najblizszyVent.transform.position;
                
                // Resetujemy cooldown z powrotem na 20 sekund
                currentCooldown = teleportCooldown;
                
                // Opcjonalnie: odtworzenie dźwięku użycia venta (tylko dla nas)
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false);
                }
            }
        }
    }
}
