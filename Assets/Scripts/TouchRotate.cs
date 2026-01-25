using UnityEngine;

public class TouchRotate : MonoBehaviour
{
    [Header("Kecepatan Putar")]
    public float rotationSpeed = 20f;

    // Fungsi bawaan Unity: Dipanggil saat Mouse/Jari menekan objek yang punya Collider
    void OnMouseDrag()
    {
        // Ambil gerakan jari horizontal (kiri-kanan)
        float rotX = Input.GetAxis("Mouse X") * rotationSpeed;
        
        // Ambil gerakan jari vertikal (atas-bawah)
        float rotY = Input.GetAxis("Mouse Y") * rotationSpeed;

        // PUTAR SUMBU Y (Biar muter kayak gasing - Kiri Kanan)
        // Pakai Space.World supaya muternya tetap tegak lurus bumi
        transform.Rotate(Vector3.up, -rotX, Space.World);

        // PUTAR SUMBU X (Biar bisa nungging - Atas Bawah)
        // Hati-hati, kadang ini bikin pusing. Kalau mau cuma kiri-kanan, hapus baris bawah ini.
        transform.Rotate(Vector3.right, rotY, Space.World);
    }
}