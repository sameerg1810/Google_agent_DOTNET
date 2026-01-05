import { Component, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common'; 
import { VoiceService } from './services/voice.service';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule], // Required for @if logic in the template
  templateUrl: './app.html', // Pointing to your existing app.html
  styleUrls: ['./app.scss']  // Pointing to your existing app.scss
})
export class App {
  private voiceService = inject(VoiceService);

  // Declare properties as signals to fix template errors
  public transcript = signal('');
  public aiResponse = signal('');
  public isListening = signal(false);
  public loading = signal(false);

  async startVoiceChat() {
    if (this.isListening()) return;

    try {
      this.isListening.set(true);
      this.loading.set(true);

      // 1. Capture user voice
      const text = await this.voiceService.listen();
      this.transcript.set(text);
      
      // 2. Get Gemini response
      const response = await firstValueFrom(this.voiceService.askGemini(text));
      this.aiResponse.set(response.reply);
      
      // 3. Play back via ElevenLabs
      this.voiceService.speak(response.reply);
      
    } catch (error) {
      console.error("Voice Error:", error);
    } finally {
      this.isListening.set(false);
      this.loading.set(false);
    }
  }
}