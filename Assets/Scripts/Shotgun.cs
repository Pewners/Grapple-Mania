using UnityEngine;
using TMPro;

public class Shotgun : MonoBehaviour
{
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;

    bool shooting, readyToShoot, reloading;

    public Camera fpsCam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask Enemy;

    public GameObject bulletHoleGraphic;
    public TextMeshProUGUI ammoText;

    public KeyCode shootKey = KeyCode.Mouse0;
    public KeyCode reloadKey = KeyCode.R;

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    private void Update()
    {
        MyInput();

        ammoText.SetText(bulletsLeft + " / " + magazineSize);
    }

    private void MyInput()
    {
        if (allowButtonHold) shooting = Input.GetKey(shootKey);
        else shooting = Input.GetKeyDown(shootKey);

        if (Input.GetKeyDown(reloadKey) && bulletsLeft < magazineSize && !reloading) Reload();

        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
            bulletsShot = bulletsPerTap;
            Shoot();
    }
    private void Shoot()
    {
        readyToShoot = false;

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 direction = fpsCam.transform.forward + new Vector3(x, y, 0);

        if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, range, Enemy))
            Debug.Log(rayHit.collider.name);

        if (rayHit.collider.CompareTag("Enemy"))
        {
            rayHit.collider.GetComponent<PlayerHit>().TakeDamage();
        }

        Instantiate(bulletHoleGraphic, rayHit.point, Quaternion.Euler(0, 180, 0));

        bulletsLeft--;
        bulletsShot--;

        Invoke("ResetShot", timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0)
        Invoke("Shoot", timeBetweenShots);
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
}