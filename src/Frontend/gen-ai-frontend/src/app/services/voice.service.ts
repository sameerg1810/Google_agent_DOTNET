import { Injectable, NgZone, Inject, PLATFORM_ID } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { isPlatformBrowser } from '@angular/common';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class VoiceService {
  private elevenLabsKey = environment.elevenLabsKey;
  private voiceId = environment.voiceId;
  private backendUrl = environment.backendUrl;

  private recognition: any;
  private isBrowser: boolean;

  constructor(
    private http: HttpClient, 
    private zone: NgZone,
    @Inject(PLATFORM_ID) platformId: Object
  ) {
    this.isBrowser = isPlatformBrowser(platformId);

    if (this.isBrowser) {
      const { webkitSpeechRecognition } = (window as any);
      if (webkitSpeechRecognition) {
        this.recognition = new webkitSpeechRecognition();
        this.recognition.lang = 'en-US';
        this.recognition.continuous = false;
      }
    }
  }

  // 1. Capture Voice (STT)
  listen(): Promise<string> {
    if (!this.isBrowser || !this.recognition) return Promise.resolve('');

    return new Promise((resolve) => {
      try {
        this.recognition.start();
        this.recognition.onresult = (event: any) => {
          const transcript = event.results[0][0].transcript;
          this.zone.run(() => resolve(transcript));
        };
        this.recognition.onerror = () => resolve('');
      } catch (e) {
        resolve('');
      }
    });
  }

  // 2. Get Gemini Response
  askGemini(prompt: string) {
    return this.http.post<{ reply: string }>(`${this.backendUrl}/ask`, { prompt });
  }

  // 3. ElevenLabs TTS with New Model & Error Handling
  speak(text: string): void {
    if (!this.isBrowser) return;

    const url = `https://api.elevenlabs.io/v1/text-to-speech/${this.voiceId}`;
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'xi-api-key': this.elevenLabsKey
    });

    const body = {
      text: text,
      // FIXED: Using 'eleven_flash_v2_5' for Free Tier support
      model_id: 'eleven_flash_v2_5', 
      voice_settings: { stability: 0.5, similarity_boost: 0.75 }
    };

    this.http.post(url, body, { headers, responseType: 'blob' }).subscribe({
      next: (blob) => {
        const audioUrl = URL.createObjectURL(blob);
        const audio = new Audio(audioUrl);
        audio.play().catch(err => console.error('Playback blocked:', err));
      },
      error: async (err: HttpErrorResponse) => {
        // FIXED: Extracting JSON error from a Blob response
        if (err.error instanceof Blob) {
          const text = await err.error.text();
          const errorDetail = JSON.parse(text);
          console.error('Detailed ElevenLabs Error:', errorDetail);
        } else {
          console.error('ElevenLabs API error:', err);
        }
      }
    });
  }
}