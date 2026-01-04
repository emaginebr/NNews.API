# Guia Rápido - Funcionalidades de IA

## 🚀 Como Começar

### 1. Iniciar o Aplicativo
```bash
cd c:\repos\NNews\nnews-app
npm run dev
```

Acesse: http://localhost:5173

---

## ✨ Criando Artigos com IA

### Passo a Passo

1. **Login** no sistema (se necessário)

2. **Vá para Articles**
   - Clique em "Articles" no menu
   - Ou acesse: http://localhost:5173/articles

3. **Clique em "Create with AI"** (botão roxo com ícone ✨)

4. **Preencha o Formulário:**

   **Description / Instructions** (obrigatório, 10-2000 caracteres):
   ```
   Escreva um artigo completo sobre React 19 e suas novas 
   funcionalidades, incluindo Server Components, Actions e 
   melhorias de performance. Adicione exemplos práticos.
   ```

   **Generate image with DALL-E 3** (opcional):
   - ☐ Não gerar imagem (mais rápido: ~3-5 segundos)
   - ☑ Gerar imagem (mais lento: ~10-15 segundos)

   **Category** (opcional):
   - Deixe vazio para IA escolher
   - Ou selecione uma categoria específica

   **Initial Status** (opcional):
   - Draft (padrão)
   - Published
   - Review
   - etc.

   **Roles** (opcional):
   ```
   public, admin
   ```

5. **Clique em "Create with AI"**

6. **Aguarde** (indicador de progresso aparece)

7. **Pronto!** Artigo criado com:
   - Título gerado pela IA
   - Conteúdo HTML completo
   - Tags sugeridas automaticamente
   - Imagem (se solicitado)

---

## 🔄 Atualizando Artigos com IA

### Passo a Passo

1. **Acesse um artigo existente**
   - Vá para Articles
   - Clique em um artigo

2. **Navegue para modo AI** (manualmente por enquanto):
   ```
   http://localhost:5173/articles/ai/123
   ```
   (substitua 123 pelo ID do artigo)

3. **Escolha o Modo:**
   - **AI Update**: Para usar IA
   - **Manual Edit**: Para editar manualmente

4. **No modo AI Update, descreva as mudanças:**

   **Exemplos de Prompts:**

   **Adicionar Conteúdo:**
   ```
   Adicione uma nova seção sobre React Server Components 
   com exemplos de código e explicações detalhadas.
   ```

   **Melhorar Qualidade:**
   ```
   Melhore a introdução tornando-a mais envolvente e 
   adicione uma conclusão resumindo os pontos principais.
   ```

   **Atualizar Informações:**
   ```
   Atualize as informações para refletir as mudanças do 
   React 19 lançado em 2024 e corrija quaisquer dados 
   desatualizados.
   ```

   **Expandir:**
   ```
   Expanda a seção de performance com mais exemplos 
   práticos e benchmarks.
   ```

   **Simplificar:**
   ```
   Simplifique a linguagem para tornar o conteúdo mais 
   acessível a iniciantes.
   ```

5. **Clique em "Update with AI"**

6. **Aguarde a atualização**

7. **Artigo atualizado!**
   - IA mantém contexto do artigo original
   - Aplica alterações solicitadas
   - Mantém coerência do conteúdo

---

## 📝 Criação Manual de Artigos

### Usando Tags (Novo Formato)

1. **Clique em "New Article"** (botão azul)

2. **Preencha o formulário:**
   - **Title**: Meu Artigo sobre React
   - **Content**: Conteúdo HTML
   - **Category**: Selecione uma categoria
   - **Tags**: Digite tags separadas por vírgula
     ```
     React, TypeScript, Tutorial, Web Development
     ```
   - **Status**: Draft / Published
   - **Roles**: admin, editor

3. **Salve**

**Importante:** Tags são criadas automaticamente se não existirem!

---

## 💡 Dicas e Truques

### Para Melhores Resultados com IA

1. **Seja Específico**
   ```
   ❌ "Escreva sobre React"
   ✅ "Escreva um tutorial completo sobre React Hooks para 
      iniciantes, incluindo useState, useEffect e custom 
      hooks com exemplos práticos"
   ```

2. **Mencione Estrutura Desejada**
   ```
   Escreva um artigo com:
   - Introdução engajante
   - 3-4 seções principais
   - Exemplos de código em cada seção
   - Conclusão com resumo dos pontos-chave
   ```

3. **Especifique Público-Alvo**
   ```
   Escreva para desenvolvedores iniciantes...
   Escreva para profissionais experientes...
   Escreva em linguagem técnica...
   Escreva em linguagem acessível...
   ```

4. **Inclua Tópicos Específicos**
   ```
   Aborde: performance, segurança, melhores práticas, 
   armadilhas comuns, casos de uso reais
   ```

### Quando Gerar Imagens

**Gere imagens quando:**
- ✅ Artigo precisa de visual atraente
- ✅ Tem tempo (10-15 segundos)
- ✅ Conteúdo é conceitual/abstrato

**Não gere imagens quando:**
- ⏱️ Precisa de velocidade
- 📝 Artigo é principalmente código
- 🎨 Tem imagem personalizada pronta

### Gerenciamento de Tags

**Tags são criadas automaticamente:**
```
"React, TypeScript, Tutorial"
```

**As tags serão criadas se não existirem:**
- React → react
- TypeScript → typescript
- Tutorial → tutorial

**Evite:**
- ❌ "React,TypeScript,Tutorial" (sem espaços - funciona mas menos legível)
- ❌ "React,, Tutorial" (vírgulas duplas)
- ❌ Tags muito longas (> 50 caracteres)

---

## 🎯 Casos de Uso Comuns

### 1. Criar Série de Tutoriais
```
Prompt: Escreva o primeiro artigo de uma série sobre React,
introduzindo conceitos básicos de componentes, props e state
para iniciantes.
```

### 2. Atualizar Artigo Antigo
```
Prompt: Atualize este artigo para React 19, substitua
class components por hooks e adicione informações sobre
Server Components.
```

### 3. Expandir Artigo Curto
```
Prompt: Expanda este artigo adicionando mais detalhes,
exemplos práticos e uma seção de troubleshooting comum.
```

### 4. Criar Guia de Referência
```
Prompt: Crie um guia de referência rápida para React Hooks
com descrição, sintaxe e exemplo de cada hook principal.
```

### 5. Artigo Comparativo
```
Prompt: Escreva uma comparação detalhada entre React e Vue.js,
destacando pontos fortes, fracos, casos de uso e quando
escolher cada framework.
```

---

## 🐛 Troubleshooting

### Erro: "Prompt must be at least 10 characters"
**Solução:** Escreva uma descrição mais detalhada (mínimo 10 caracteres)

### Erro: "Failed to generate article"
**Possíveis causas:**
- Backend não está rodando
- Sem autenticação válida
- API key do OpenAI não configurada no backend
- Timeout (prompt muito complexo)

**Solução:**
1. Verifique se o backend está rodando
2. Verifique autenticação (token válido)
3. Tente prompt mais simples
4. Verifique logs do backend

### Geração Muito Lenta
**Causas:**
- Geração de imagem ativada (adiciona 5-10 segundos)
- Prompt muito complexo
- API do OpenAI com latência

**Solução:**
- Desative geração de imagem se não necessária
- Simplifique o prompt
- Aguarde pacientemente (máximo ~15 segundos)

### Tags Não Aparecem
**Solução:**
- Tags são criadas automaticamente ao salvar
- Verifique formato: "Tag1, Tag2, Tag3"
- Sem vírgulas duplas: ~~"Tag1,, Tag2"~~

---

## 📊 Comparação: Manual vs IA

| Aspecto | Manual | IA |
|---------|--------|-----|
| **Velocidade** | Varia (minutos a horas) | 3-15 segundos |
| **Qualidade** | Depende do autor | Consistente, profissional |
| **Criatividade** | Alta personalização | Baseada no prompt |
| **Tags** | Manual | Sugeridas automaticamente |
| **Imagens** | Upload manual | Geradas (DALL-E 3) |
| **Ideal para** | Conteúdo muito específico | Conteúdo padrão, rascunhos |

---

## 🎓 Exemplos de Prompts Efetivos

### Tutorial Técnico
```
Escreva um tutorial completo sobre como criar uma API REST
com Node.js e Express. Inclua:
- Setup inicial do projeto
- Criação de rotas CRUD
- Conexão com banco de dados
- Tratamento de erros
- Testes com exemplos de código
```

### Artigo de Opinião
```
Escreva um artigo argumentando sobre as vantagens do
TypeScript sobre JavaScript vanilla para projetos grandes,
incluindo exemplos práticos e casos de uso reais.
```

### Guia de Boas Práticas
```
Crie um guia de boas práticas de segurança para aplicações
React, cobrindo: XSS, CSRF, autenticação, autorização,
sanitização de inputs e exemplos de vulnerabilidades comuns.
```

### Comparativo
```
Compare React, Vue e Angular em termos de performance,
curva de aprendizado, ecossistema, casos de uso ideais e
suporte da comunidade. Adicione tabela comparativa.
```

### Artigo Notícia
```
Escreva sobre as novidades do React 19 lançado em 2024,
destacando React Server Components, Actions, e melhorias
de performance com exemplos de migração.
```

---

## 🔗 Links Úteis

- **Documentação nnews-react**: `nnews-react/CHANGELOG.md`
- **Guia de Features**: `nnews-react/AI_FEATURES_GUIDE.md`
- **Exemplos de Código**: `nnews-react/examples/`
- **Backend API**: http://localhost:5000/swagger

---

**Última Atualização:** 3 de Janeiro de 2026  
**Versão:** 2.0.0
