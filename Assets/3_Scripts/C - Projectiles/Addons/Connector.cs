using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Laser))]
public class Connector : ProjectileAddon
{
    private bool connectorActive;

    // settings
    [Range(1, 2)] public int maxConnections = 1;
    public float maxConnectionRange = 5;

    // connection
    public ConnectionMode connectionTarget = ConnectionMode.SameProjectilesSamePlayer;
    public enum ConnectionMode
    {
        SameProjectilesSamePlayer,
        SameProjectiles,
        AllEnemies
    }

    public ConnectionPreference connectionPreference = ConnectionPreference.InSpawnOrder;
    public enum ConnectionPreference
    {
        InSpawnOrder,
        Closest
    }

    // references
    public LayerMask whatIsProjectile;
    public LayerMask whatIsEnemy;

    private Projectile projectile;
    private Laser laser;

    private List<Connector> connections = new List<Connector>();
    private int externalConnections; // connections controlled by older Connectors

    private void Start()
    {
        projectile = GetComponent<Projectile>();
        laser = GetComponent<Laser>();
        laser.StopLaser();

        ActivateConnector();
    }

    public void ActivateConnector()
    {
        connectorActive = true;
    }

    public void DeactivateConnector()
    {
        connectorActive = false;
    }

    private void Update()
    {
        if (!connectorActive) return;

        CheckActiveConnections();

        // return when no connections are possible
        if (ConnectionAmount() >= maxConnections) return;

        // check for new projectile connections
        if(connectionTarget == ConnectionMode.SameProjectilesSamePlayer || connectionTarget == ConnectionMode.SameProjectiles)
            CheckForProjectileConnections();

        // check for new enemy connections
        if (connectionTarget == ConnectionMode.AllEnemies)
            CheckForEnemyConnections();
    }

    #region CheckConnections

    private void CheckForProjectileConnections()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, maxConnectionRange, whatIsProjectile);

        // random for now
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].TryGetComponent(out Connector connector))
                TryConnectTo(connector);
        }
    }

    private Connector GetNextSpawnedConnector()
    {
        throw new System.NotImplementedException();
    }

    private Connector GetClosestConnector()
    {
        throw new System.NotImplementedException();
    }

    private void CheckForEnemyConnections()
    {

    }

    private void CheckActiveConnections()
    {
        // I guess this could work since it's running every frame?
        for (int i = 0; i < connections.Count; i++)
        {
            if (connections[i] == null)
            {
                connections.RemoveAt(i);
                laser.StopLaser();
                connections[i].RemoveExternalConnection();
                continue;
            }

            if (Vector3.Distance(transform.position, connections[i].transform.position) > maxConnectionRange) 
            {
                connections.RemoveAt(i);
                laser.StopLaser();
                connections[i].RemoveExternalConnection();
            }
        }
    }

    #endregion

    #region Connect

    private void TryConnectTo(Connector connector)
    {
        if (connector == null) return;

        // Check 0 - can't connect to self
        if (connector == this) return;

        // Check 1 - any connections left?
        if (ConnectionAmount() >= maxConnections) return;

        // Check 2 - is connector older?
        if (connector.GetSpawnTime() < GetSpawnTime()) return;

        print(gameObject.name + GetSpawnTime() + " wants to connect to " + connector.gameObject.name + connector.GetSpawnTime());
        
        laser.StartLaser(connector.transform);
        connector.AddExternalConnection();

        // connection complete
        connections.Add(connector);
    }

    public void AddExternalConnection()
    {
        externalConnections++;
    }

    public void RemoveExternalConnection()
    {

    }

    #endregion

    /// older one connects to newer one
    /// if max connections are full this can be ignored
    /// 

    #region Getters

    public float GetSpawnTime()
    {
        if (projectile != null) return projectile.spawnTime;
        else return 0f;
    }

    private int ConnectionAmount()
    {
        return connections.Count + externalConnections;
    }

    #endregion
}
