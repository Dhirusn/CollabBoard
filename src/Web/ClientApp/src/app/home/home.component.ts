import { Component } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent {
  faqs = [
    { question: 'Does Basewell AI train on customer data?', answer: 'No, Basewell does not use your data for model training.' },
    { question: 'Does Basewell offer migration services?', answer: 'Yes, we provide seamless migration support.' },
    { question: 'What mobile features are supported?', answer: 'All features are responsive and available on mobile.' },
    { question: 'Do I need an LMS to use Basewell?', answer: 'No LMS required, Basewell works standalone.' },
    { question: 'Can I try Basewell for free?', answer: 'Yes, a free trial is available.' },
    { question: 'How do I contact support?', answer: 'Support is accessible via email or live chat.' },
    { question: 'Do you offer SLAs?', answer: 'Yes, enterprise plans come with SLA options.' }
  ];

   features = [
    {
      icon: 'bi-plug',
      title: 'Integrations',
      description: 'Sync training and ask questions from apps you’re already using'
    },
    {
      icon: 'bi-people',
      title: 'User Groups',
      description: 'Easily bring people, content, and data together'
    },
    {
      icon: 'bi-award',
      title: 'Certifications',
      description: 'Automatically certify once training has been completed'
    },
    {
      icon: 'bi-journal-bookmark',
      title: 'Course Library',
      description: 'Navigate knowledge, training, and essential content, anytime'
    },
    {
      icon: 'bi-arrow-repeat',
      title: 'Directory Sync',
      description: 'Automate critical admin tasks based on HRIS workflows'
    },
    {
      icon: 'bi-lightbulb',
      title: 'Knowledge Base',
      description: 'Structure a living wiki without needing other tools'
    }
  ];

  cards = [
    {
      title: 'Instant support',
      description: 'Employee questions are answered in seconds',
      image: 'assets/instant-support.png' // replace with your actual image
    },
    {
      title: 'Self-maintaining',
      description: 'Intelligence updates in real time as info changes',
      image: 'assets/self-maintaining.png'
    },
    {
      title: 'Universally compatible',
      description: 'Answers blend from multiple sources',
      image: 'assets/universally-compatible.png'
    }
  ];
}
