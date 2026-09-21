using UnityEngine;
using System.Collections.Generic;

public class EntityFootStep : MonoBehaviour
{
    public AudioSource footstepAudioSource; // 발소리 재생을 위한 AudioSource
    public List<AudioClip> footstepClips; // 발소리 오디오 클립

    // 매개변수가 없는 이벤트 함수
    public void OnFootstep()
    {
        // 발소리 사운드 재생 로직
        footstepAudioSource.PlayOneShot(footstepClips[Random.Range(0, footstepClips.Count)]);
    }

}
