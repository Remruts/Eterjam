using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CamScript : MonoBehaviour
{
  public static CamScript screen;  

  bool shaking = false;
  float intensity = 0f;
  float shakeIncrement = 1f;

  Vector3 currentPos;
  Vector3 shakenPos;
  //public Transform target;
  public Vector3 offset;

  public Rect mapBounds = new Rect(-13f, -7f, 26f, 14.3f);
  public float horizontalZoomZoneSize = 1f;
  public float verticalZoomZoneSize = 1f;
  public float minZoom = 4f;
  float minHorizontalZoom;
  float maxZoom;
  public float horizontalDeadZoneSize = 1f;
  public float verticalDeadZoneSize = 1f;

  float camVerticalSize;
  float camHorizontalSize;

  Vector3 midPoint;

  List<GameObject> targets;

  public bool followAndZoom = true;

  float targetsMinX;
  float targetsMaxX;
  float targetsMinY;
  float targetsMaxY;

  void Awake(){
    screen = this;    
    currentPos = transform.position;

    midPoint = transform.position;
  }

  void Update(){
    if (shaking){
      intensity += shakeIncrement * Time.deltaTime;
    }

    if (!shaking && intensity > 0){
      intensity -= shakeIncrement * Time.deltaTime;

      if (intensity < 0){
        intensity = 0;
      }
    }

    minHorizontalZoom = minZoom * Camera.main.pixelWidth / Camera.main.pixelHeight;

    maxZoom = (mapBounds.yMax - mapBounds.yMin);

    camVerticalSize = Camera.main.orthographicSize;
    camHorizontalSize = camVerticalSize * Camera.main.pixelWidth / Camera.main.pixelHeight;

    targets = MatchManager.match.getPlayers();
  }

  // Update is called once per frame
  void LateUpdate(){
    if (MatchManager.match != null && MatchManager.match.paused){
      return;
    }

    if (followAndZoom){
      // Move to midpoint of all targets (players)
      moveToCenter();

      // Zoom to fit all targets (players)
      zoomToFit();
    }

    if (followAndZoom){
      // limit camera position
      limitPosition();
    }

    // Shaking.
    if (intensity > 0f){
      shakenPos.x = currentPos.x + getShake();
      shakenPos.y = currentPos.y + getShake();
      shakenPos.z = currentPos.z;

      transform.position = shakenPos + offset;
    } else {
      transform.position = Vector3.Lerp(transform.position, currentPos + offset,Time.deltaTime * 10f);
    }    

  }

  void moveToCenter(){
    // Get the midpoint of the targets
    midPoint = Vector3.zero;
    int playernums = 0;
    foreach (var target in targets){
      if (target != null){
        Vector3 targetPos = target.transform.position;
        midPoint += targetPos;
        playernums += 1;
      }
    }

    midPoint /= playernums;

    currentPos += (midPoint - currentPos) * Time.deltaTime * 10f;
  }

  void zoomToFit(){

    camVerticalSize = Camera.main.orthographicSize;
    camHorizontalSize = camVerticalSize * Camera.main.pixelWidth / Camera.main.pixelHeight;

    targetsMinX = mapBounds.xMax;
    targetsMaxX = mapBounds.xMin;
    targetsMinY = mapBounds.yMax;
    targetsMaxY = mapBounds.yMin;

    float leftBound = transform.position.x - camHorizontalSize;
    float rightBound = transform.position.x + camHorizontalSize;
    float bottomBound = transform.position.y - camVerticalSize;
    float topBound = transform.position.y + camVerticalSize;

    foreach (var target in targets){
      Vector3 targetPos = target.transform.position;
      
      if (targetPos.x < targetsMinX){
        targetsMinX = targetPos.x;
      }
      if (targetPos.x > targetsMaxX){
        targetsMaxX = targetPos.x;
      }
      if (targetPos.y < targetsMinY){
        targetsMinY = targetPos.y;
      }
      if (targetPos.y > targetsMaxY){
        targetsMaxY = targetPos.y;
      }
    }

    float newLeftBound = leftBound;
    float newRightBound = rightBound;
    float newBottomBound = bottomBound;
    float newTopBound = topBound;

     // Zoom Out
    if (targetsMinX < leftBound + horizontalZoomZoneSize){
      newLeftBound = leftBound - horizontalZoomZoneSize * 2.0f;
    }
    if (targetsMaxX > rightBound - horizontalZoomZoneSize){
      newRightBound = rightBound + horizontalZoomZoneSize * 2.0f;
    }
    if (targetsMinY < bottomBound + verticalZoomZoneSize){
      newBottomBound = bottomBound - verticalZoomZoneSize * 2.0f;
    }
    if (targetsMaxY > topBound - verticalZoomZoneSize){
      newTopBound = topBound + verticalZoomZoneSize * 2.0f;
    }

    // Zoom In    
    if (targetsMinX > leftBound + horizontalZoomZoneSize + horizontalDeadZoneSize){
      newLeftBound = leftBound + horizontalZoomZoneSize;
    }
    if (targetsMaxX < rightBound - horizontalZoomZoneSize - horizontalDeadZoneSize){
      newRightBound = rightBound - horizontalZoomZoneSize;
    }
    if (targetsMinY > bottomBound + verticalZoomZoneSize + verticalDeadZoneSize){
      newBottomBound = bottomBound + verticalZoomZoneSize;
    }
    if (targetsMaxY < topBound - verticalZoomZoneSize - verticalDeadZoneSize){
      newTopBound = topBound - verticalZoomZoneSize;
    }

    // Clamp to bounds
    newTopBound = Mathf.Min(newTopBound, mapBounds.yMax);
    newBottomBound = Mathf.Max(newBottomBound, mapBounds.yMin);
    newRightBound = Mathf.Min(newRightBound, mapBounds.xMax);
    newLeftBound = Mathf.Max(newLeftBound, mapBounds.xMin);
    
    //Debug.Log($"topBound: {topBound}, bottomBound: {bottomBound}, rightBound: {rightBound}, leftBound: {leftBound}");

    float sizeX_Vertical = (newRightBound - newLeftBound) / 2.0f * Camera.main.pixelHeight / Camera.main.pixelWidth;
    float sizeY = (newTopBound - newBottomBound) / 2.0f;
    
    //Debug.Log($"sizeX_Vertical: {sizeX_Vertical}, sizeY: {sizeY}");
    
    float camSize = (sizeX_Vertical > sizeY ? sizeX_Vertical : sizeY);
    camSize = Mathf.Clamp(camSize, minZoom, maxZoom);

    Camera.main.orthographicSize = Mathf.Lerp(camVerticalSize, camSize, Time.deltaTime * 5f);
  }

  void limitPosition(){
    camVerticalSize = Camera.main.orthographicSize;
    camHorizontalSize = camVerticalSize * Camera.main.pixelWidth / Camera.main.pixelHeight;

    currentPos.x = Mathf.Clamp(currentPos.x, mapBounds.xMin + camHorizontalSize - offset.x, mapBounds.xMax - camHorizontalSize - offset.x);
    currentPos.y = Mathf.Clamp(currentPos.y, mapBounds.yMin + camVerticalSize - offset.y, mapBounds.yMax - camVerticalSize - offset.y);
    
    if (mapBounds.width / 2.0f <= camHorizontalSize){
      currentPos.x = (mapBounds.xMin + mapBounds.xMax)/2.0f;
    }
    if (mapBounds.height / 2.0f <= camVerticalSize){
      currentPos.y = (mapBounds.yMin + mapBounds.yMax)/2.0f;
    }
    
    currentPos.z = -10f;
  }

  public void jumpTo(Vector3 pos){
    currentPos = pos;
    currentPos.z = -10f;
    transform.position = currentPos + offset;
  }
  float getShake(){
    float shakingNumber = Random.Range(-intensity / 2f, intensity / 2f);
    shakingNumber += (Mathf.Sign(shakingNumber) * intensity / 2f);

    return shakingNumber;
  }

  public void shake(float time, float factor){
    shaking = true;

    if (time > 0){
      Invoke("stopShaking", time);
    }

    shakeIncrement = factor;
  }

  public void stopShaking(){
    shaking = false;
  }

   void OnDrawGizmosSelected(){
    if (mapBounds == null){
      return;
    }
    // Draw the map bounds
    Gizmos.color = Color.red;

    Gizmos.DrawWireCube(new Vector2((mapBounds.xMin + mapBounds.xMax) / 2.0f, (mapBounds.yMin + mapBounds.yMax) / 2.0f), new Vector3(mapBounds.width, mapBounds.height, 1f));

    if (!followAndZoom){
      return;
    }

    // Draw the minimum zoom
    Gizmos.color = Color.yellow;

    minHorizontalZoom = minZoom * Camera.main.pixelWidth / Camera.main.pixelHeight;

    Gizmos.DrawWireCube(new Vector2(transform.position.x, transform.position.y), new Vector3(minHorizontalZoom * 2.0f, minZoom * 2.0f, 1f));

    // Draw the zoom zones
    Gizmos.color = new Color(0.5f, 0.32f, 0.73f, 0.4f);
    
    camVerticalSize = Camera.main.orthographicSize;
    camHorizontalSize = camVerticalSize * Camera.main.pixelWidth / Camera.main.pixelHeight;
    
    // left zoom zone
    Gizmos.DrawCube(
      new Vector2(
        (transform.position.x - camHorizontalSize) + horizontalZoomZoneSize/2.0f,
        transform.position.y)
      , new Vector3(horizontalZoomZoneSize, camVerticalSize * 2.0f, 1f)
    );

    // right zoom zone
    Gizmos.DrawCube(
      new Vector2(
        (transform.position.x + camHorizontalSize) - horizontalZoomZoneSize/2.0f,
        transform.position.y)
      , new Vector3(horizontalZoomZoneSize, camVerticalSize * 2.0f, 1f)
    );

    // top zoom zone
    Gizmos.DrawCube(
      new Vector2(
        transform.position.x,
        (transform.position.y + camVerticalSize) - verticalZoomZoneSize/2.0f)
      , new Vector3((camHorizontalSize - horizontalZoomZoneSize) * 2.0f, verticalZoomZoneSize, 1f)
    );

    // bottom zoom zone
    Gizmos.DrawCube(
      new Vector2(
        transform.position.x,
        (transform.position.y - camVerticalSize) + verticalZoomZoneSize/2.0f)
      , new Vector3((camHorizontalSize - horizontalZoomZoneSize) * 2.0f, verticalZoomZoneSize, 1f)
    );

    // draw the dead zones
    Gizmos.color = new Color(1f, 1f, 0f, 0.4f);
    // left Deadzone 
    Gizmos.DrawCube(
      new Vector2(
        (transform.position.x - camHorizontalSize) + horizontalZoomZoneSize + horizontalDeadZoneSize/2.0f,
        transform.position.y)
      , new Vector3(horizontalDeadZoneSize, (camVerticalSize - verticalZoomZoneSize) * 2.0f, 1f)
    );

    // right Deadzone
    Gizmos.DrawCube(
      new Vector2(
        (transform.position.x + camHorizontalSize) - horizontalZoomZoneSize - horizontalDeadZoneSize/2.0f,
        transform.position.y)
      , new Vector3(horizontalDeadZoneSize, (camVerticalSize - verticalZoomZoneSize) * 2.0f, 1f)
    );

    // top Deadzone
    Gizmos.DrawCube(
      new Vector2(
        transform.position.x,
        (transform.position.y + camVerticalSize) - verticalZoomZoneSize - verticalDeadZoneSize/2.0f)
      , new Vector3((camHorizontalSize - horizontalZoomZoneSize - horizontalDeadZoneSize) * 2.0f, verticalDeadZoneSize, 1f)
    );

    // bottom Deadzone
    Gizmos.DrawCube(
      new Vector2(
        transform.position.x,
        (transform.position.y - camVerticalSize) + verticalZoomZoneSize + verticalDeadZoneSize/2.0f)
      , new Vector3((camHorizontalSize - horizontalZoomZoneSize - horizontalDeadZoneSize) * 2.0f, verticalDeadZoneSize, 1f)
    );
   }

}
