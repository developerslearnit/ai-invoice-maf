// tools/deck-generator/build-deck.js
const pptxgen = require('pptxgenjs');
const path = require('path');
const fs = require('fs');

console.log('Initializing PowerPoint Presentation Generator...');

const pres = new pptxgen();
pres.layout = 'LAYOUT_WIDE'; // Standard modern 16:9 widescreen (13.333 x 7.5 inches)
pres.author = 'Adesina Mark Omoniyi';
pres.company = 'Microsoft Agent Framework Training';
pres.title = 'Integrating AI into Existing .NET Applications with Microsoft Agent Framework';
pres.subject = 'From Traditional ASP.NET Core Applications to Agentic AI Systems';

// Theme Palette
const COLORS = {
  NAVY_DARK: '0F172A',
  SLATE_CARD: '1E293B',
  LIGHT_BG: 'F8FAFC',
  WHITE: 'FFFFFF',
  AZURE_BLUE: '0078D4',
  AZURE_LIGHT: 'E0F2FE',
  AZURE_BORDER: 'BAE6FD',
  SKY_BLUE: '0284C7',
  TEXT_DARK: '0F172A',
  TEXT_MUTED: '64748B',
  BORDER_LIGHT: 'E2E8F0',
  CODE_BG: '1E1E1E',
  CODE_TEXT: 'D4D4D4',
  CODE_GREEN: '4EC9B0',
  CODE_KEYWORD: '569CD6',
  SUCCESS_GREEN: '107C41',
  SUCCESS_BG: 'DCFCE7',
  WARNING_ORANGE: 'D83B01',
  WARNING_BG: 'FFEDD5',
  DEMO_RED: 'B91C1C',
  DEMO_BG: 'FEE2E2',
  CARD_BG: 'FFFFFF'
};

const FONTS = {
  TITLE: 'Segoe UI',
  BODY: 'Segoe UI',
  CODE: 'Consolas'
};

// Helper: Standard Header for Instructional Slides
function addSlideHeader(slide, moduleTag, title, subtitle) {
  // Module Tag Badge
  slide.addShape(pres.ShapeType.roundRect, {
    x: 0.8, y: 0.45, w: 2.8, h: 0.32,
    rectRadius: 0.1,
    fill: { color: COLORS.AZURE_LIGHT },
    line: { color: COLORS.AZURE_BORDER, width: 1 }
  });
  slide.addText(moduleTag.toUpperCase(), {
    x: 0.8, y: 0.45, w: 2.8, h: 0.32,
    fontSize: 9, bold: true, color: COLORS.SKY_BLUE,
    fontFace: FONTS.BODY, align: 'center', valign: 'middle'
  });

  // Slide Title
  slide.addText(title, {
    x: 0.8, y: 0.82, w: 11.7, h: 0.55,
    fontSize: 22, bold: true, color: COLORS.TEXT_DARK,
    fontFace: FONTS.TITLE, valign: 'top'
  });

  // Slide Subtitle
  if (subtitle) {
    slide.addText(subtitle, {
      x: 0.8, y: 1.35, w: 11.7, h: 0.35,
      fontSize: 12, color: COLORS.TEXT_MUTED,
      fontFace: FONTS.BODY, valign: 'top'
    });
  }

  // Divider Line
  slide.addShape(pres.ShapeType.line, {
    x: 0.8, y: 1.72, w: 11.73, h: 0,
    line: { color: COLORS.BORDER_LIGHT, width: 1 }
  });
}

// Helper: Header for Hands-On Coding Lab Slides
function addCodingLabHeader(slide, labNumber, labTitle, objective) {
  // Emerald Coding Lab Badge
  slide.addShape(pres.ShapeType.roundRect, {
    x: 0.8, y: 0.45, w: 3.4, h: 0.34,
    rectRadius: 0.1,
    fill: { color: COLORS.SUCCESS_GREEN },
    line: { color: '059669', width: 1 }
  });
  slide.addText(`★ HANDS-ON CODING LAB ${labNumber}`.toUpperCase(), {
    x: 0.8, y: 0.45, w: 3.4, h: 0.34,
    fontSize: 9.5, bold: true, color: COLORS.WHITE,
    fontFace: FONTS.BODY, align: 'center', valign: 'middle'
  });

  // Lab Title
  slide.addText(labTitle, {
    x: 0.8, y: 0.82, w: 11.7, h: 0.55,
    fontSize: 22, bold: true, color: COLORS.TEXT_DARK,
    fontFace: FONTS.TITLE, valign: 'top'
  });

  // Objective Subtitle
  if (objective) {
    slide.addText(`LIVE CODING GOAL: ${objective}`, {
      x: 0.8, y: 1.35, w: 11.7, h: 0.35,
      fontSize: 11.5, bold: true, color: COLORS.AZURE_BLUE,
      fontFace: FONTS.BODY, valign: 'top'
    });
  }

  // Divider Line
  slide.addShape(pres.ShapeType.line, {
    x: 0.8, y: 1.72, w: 11.73, h: 0,
    line: { color: '10B981', width: 1.5 }
  });
}

// Helper: Standard Slide Footer
function addSlideFooter(slide, currentSlide, totalSlides = 70) {
  slide.addText('Integrating AI into Existing .NET Applications with Microsoft Agent Framework | Adesina Mark Omoniyi', {
    x: 0.8, y: 6.95, w: 9.5, h: 0.28,
    fontSize: 9, color: COLORS.TEXT_MUTED, fontFace: FONTS.BODY
  });
  slide.addText(`${currentSlide} / ${totalSlides}`, {
    x: 10.8, y: 6.95, w: 1.73, h: 0.28,
    fontSize: 9, color: COLORS.TEXT_MUTED, fontFace: FONTS.BODY, align: 'right'
  });
}

// Helper: Formatter for Speaker Notes
function formatNotes(data) {
  const parts = [];
  if (data.explanation) {
    parts.push(`EXPLANATION:\n${data.explanation}`);
  }
  if (data.whatToEmphasize) {
    parts.push(`WHAT TO EMPHASIZE:\n${data.whatToEmphasize}`);
  }
  if (data.discussionPrompt) {
    parts.push(`DISCUSSION PROMPT:\n${data.discussionPrompt}`);
  }
  if (data.demoGuidance) {
    parts.push(`DEMO GUIDANCE:\n${data.demoGuidance}`);
  }
  if (data.implementationCaveats) {
    parts.push(`IMPLEMENTATION CAVEATS:\n${data.implementationCaveats}`);
  }
  if (data.commonStudentMisconception) {
    parts.push(`COMMON STUDENT MISCONCEPTION:\n${data.commonStudentMisconception}`);
  }
  if (data.transition) {
    parts.push(`TRANSITION:\n${data.transition}`);
  }
  return parts.join('\n\n');
}

module.exports = {
  pres,
  COLORS,
  FONTS,
  addSlideHeader,
  addCodingLabHeader,
  addSlideFooter,
  formatNotes
};
