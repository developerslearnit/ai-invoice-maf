// tools/deck-generator/index.js
const { pres } = require('./build-deck');
const { createPart1Slides } = require('./slides-part1');
const { createPart2Slides } = require('./slides-part2');
const { createPart3Slides } = require('./slides-part3');
const { createPart4Slides } = require('./slides-part4');
const path = require('path');
const fs = require('fs');

async function main() {
  console.log('====================================================');
  console.log('Generating Enterprise AI Presentation with pptxgenjs...');
  console.log('Course: Integrating AI into Existing .NET Applications with Microsoft Agent Framework');
  console.log('Instructor: Adesina Mark Omoniyi');
  console.log('====================================================');

  // Build all 58 slides in sequence
  createPart1Slides(pres);
  createPart2Slides(pres);
  createPart3Slides(pres);
  createPart4Slides(pres);

  const outputDir = path.resolve(__dirname, '../../output/presentations');
  if (!fs.existsSync(outputDir)) {
    fs.mkdirSync(outputDir, { recursive: true });
  }

  const outputPath = path.join(outputDir, 'invoiceai-microsoft-agent-framework-course.pptx');
  console.log(`Writing presentation to: ${outputPath}...`);

  await pres.writeFile({ fileName: outputPath });

  const stats = fs.statSync(outputPath);
  console.log('====================================================');
  console.log(`Presentation successfully created!`);
  console.log(`File: ${outputPath}`);
  console.log(`Size: ${(stats.size / (1024 * 1024)).toFixed(2)} MB`);
  console.log(`Total Slides: 70`);
  console.log('====================================================');
}

main().catch(err => {
  console.error('Error generating presentation:', err);
  process.exit(1);
});
